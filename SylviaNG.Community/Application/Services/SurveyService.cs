using Microsoft.Extensions.Logging;
using SylviaNG.Community.Application.Common.Exceptions;
using SylviaNG.Community.Application.Features.Notifications.Models;
using SylviaNG.Community.Application.Features.Surveys;
using SylviaNG.Community.Application.Features.Surveys.Models;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.Application.Mappings;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;
using SylviaNG.Community.SharedKernel.Pagination;
// Domain.Entities currently also defines a "Task" entity (unrelated task-management module);
// alias here so the async method return type below resolves unambiguously.
using Task = System.Threading.Tasks.Task;

namespace SylviaNG.Community.Application.Services
{
    public class SurveyService : ISurveyService
    {
        private readonly ISurveyRepository _surveyRepository;
        private readonly ISurveyAudienceRepository _surveyAudienceRepository;
        private readonly ISurveyQuestionRepository _surveyQuestionRepository;
        private readonly ISurveyOptionRepository _surveyOptionRepository;
        private readonly ISurveyResponseRepository _surveyResponseRepository;
        private readonly ISurveyAnswerRepository _surveyAnswerRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly INotificationService _notificationService;
        private readonly ILogger<SurveyService> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public SurveyService(
            ISurveyRepository surveyRepository,
            ISurveyAudienceRepository surveyAudienceRepository,
            ISurveyQuestionRepository surveyQuestionRepository,
            ISurveyOptionRepository surveyOptionRepository,
            ISurveyResponseRepository surveyResponseRepository,
            ISurveyAnswerRepository surveyAnswerRepository,
            IEmployeeRepository employeeRepository,
            INotificationService notificationService,
            ILogger<SurveyService> logger,
            IUnitOfWork unitOfWork)
        {
            _surveyRepository = surveyRepository;
            _surveyAudienceRepository = surveyAudienceRepository;
            _surveyQuestionRepository = surveyQuestionRepository;
            _surveyOptionRepository = surveyOptionRepository;
            _surveyResponseRepository = surveyResponseRepository;
            _surveyAnswerRepository = surveyAnswerRepository;
            _employeeRepository = employeeRepository;
            _notificationService = notificationService;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(SurveyCreateRequest request)
        {
            var exists = await _surveyRepository.ExistsByTitleAsync(request.Title);
            if (exists)
                throw new DuplicateException("Survey", "Title", request.Title);

            var entity = request.ToEntity();
            await _surveyRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.SurveyId;
        }

        public async Task UpdateAsync(long surveyId, SurveyUpdateRequest request)
        {
            var entity = await _surveyRepository.GetByIdAsync(surveyId)
                ?? throw new NotFoundException("Survey", surveyId);

            if (entity.Status == "Closed")
            {
                throw new FluentValidation.ValidationException(new[]
                {
                    new FluentValidation.Results.ValidationFailure(nameof(entity.Status), "Closed surveys cannot be edited.")
                });
            }

            entity.ApplyUpdate(request);
            _surveyRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task PublishAsync(long surveyId)
        {
            var entity = await _surveyRepository.GetByIdAsync(surveyId)
                ?? throw new NotFoundException("Survey", surveyId);

            if (entity.Status != "Draft")
            {
                throw new FluentValidation.ValidationException(new[]
                {
                    new FluentValidation.Results.ValidationFailure(nameof(entity.Status),
                        $"Only Draft surveys can be published (current status: {entity.Status}).")
                });
            }

            if (string.IsNullOrEmpty(entity.ExternalUrl))
            {
                var questions = await _surveyQuestionRepository.GetBySurveyIdAsync(surveyId);
                if (questions.Count == 0)
                {
                    throw new FluentValidation.ValidationException(new[]
                    {
                        new FluentValidation.Results.ValidationFailure("Questions",
                            "A survey must have at least one question before it can be published (unless it links to an ExternalUrl).")
                    });
                }
            }

            entity.Status = "Published";
            entity.PublishedAt = DateTime.UtcNow;
            _surveyRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();

            var recipientIds = await GetEligibleEmployeeIdsAsync(surveyId);
            foreach (var employeeId in recipientIds)
            {
                await NotifySurveyPublishedSafeAsync(employeeId, entity);
            }
        }

        /// <summary>
        /// Resolves which employees are eligible for a survey's audience - used to decide who gets
        /// notified on publish, who sees it in their eligible-surveys list, who may load its detail/
        /// questions, and who may submit a response. Mirrors GetResultsAsync's audience branching
        /// (EntireCompany vs Department/Branch) below, but unlike there - where "no rows" means an
        /// unknown participation-rate denominator - every other caller treats no audience rows and an
        /// EntireCompany row identically: eligible to every active employee.
        /// </summary>
        private async Task<HashSet<long>> GetEligibleEmployeeIdsAsync(long surveyId)
        {
            var audience = await _surveyAudienceRepository.GetBySurveyIdAsync(surveyId);

            if (audience.Count == 0 || audience.Any(a => a.AudienceType == SurveyAudienceTypes.EntireCompany))
                return (await _employeeRepository.GetActiveIdsAsync()).ToHashSet();

            var departmentIds = audience
                .Where(a => a.AudienceType == SurveyAudienceTypes.Department && a.DepartmentId.HasValue)
                .Select(a => a.DepartmentId!.Value)
                .ToList();
            var siteIds = audience
                .Where(a => a.AudienceType == SurveyAudienceTypes.Branch && a.BranchId.HasValue)
                .Select(a => a.BranchId!.Value)
                .ToList();

            var recipientIds = new HashSet<long>();
            if (departmentIds.Count > 0)
                recipientIds.UnionWith(await _employeeRepository.GetActiveIdsByDepartmentIdsAsync(departmentIds));
            if (siteIds.Count > 0)
                recipientIds.UnionWith(await _employeeRepository.GetActiveIdsBySiteIdsAsync(siteIds));

            return recipientIds;
        }

        /// <summary>
        /// Isolates notification delivery from the publish operation it follows - the survey is
        /// already durably Published by the time this runs, and with the default recipient set
        /// being potentially every active employee company-wide, one failure shouldn't turn an
        /// already-successful publish into a 500 or stop the remaining recipients from being
        /// notified. Mirrors PollService.BroadcastPollResultsSafeAsync's same "isolate a
        /// non-critical side effect" shape.
        /// </summary>
        private async Task NotifySurveyPublishedSafeAsync(long employeeId, Survey survey)
        {
            try
            {
                await _notificationService.CreateAsync(new NotificationCreateRequest
                {
                    EmployeeId = employeeId,
                    Title = $"New survey published: {survey.Title}",
                    Category = "Survey",
                    RelatedEntityType = "Survey",
                    RelatedEntityId = survey.SurveyId
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to notify employee {EmployeeId} about published survey {SurveyId}", employeeId, survey.SurveyId);
            }
        }

        public async Task CloseAsync(long surveyId)
        {
            var entity = await _surveyRepository.GetByIdAsync(surveyId)
                ?? throw new NotFoundException("Survey", surveyId);

            if (entity.Status != "Published")
            {
                throw new FluentValidation.ValidationException(new[]
                {
                    new FluentValidation.Results.ValidationFailure(nameof(entity.Status),
                        $"Only Published surveys can be closed (current status: {entity.Status}).")
                });
            }

            entity.Status = "Closed";
            entity.ClosedAt = DateTime.UtcNow;
            _surveyRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(long surveyId)
        {
            var entity = await _surveyRepository.GetByIdAsync(surveyId)
                ?? throw new NotFoundException("Survey", surveyId);

            if (entity.Status == "Closed")
            {
                throw new FluentValidation.ValidationException(new[]
                {
                    new FluentValidation.Results.ValidationFailure(nameof(entity.Status), "Closed surveys cannot be deleted.")
                });
            }

            var questionIds = (await _surveyQuestionRepository.GetBySurveyIdAsync(surveyId))
                .Select(q => q.QuestionId)
                .ToList();

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                // SurveyAnswer.QuestionId is a Restrict FK - SurveyQuestion already cascades from
                // Survey, and cascading SurveyAnswer from both Survey->SurveyQuestion and
                // Survey->SurveyResponse would create multiple cascade paths to the same table,
                // which SQL Server rejects at schema-creation time (see SurveyAnswerConfiguration).
                // Under Postgres this means a single Survey delete can attempt the SurveyQuestion
                // cascade before the separate SurveyResponse->SurveyAnswer cascade removes the
                // answers still referencing those questions, violating that Restrict FK. Deleting
                // the answers explicitly first, as its own statement, avoids the ordering conflict.
                if (questionIds.Count > 0)
                {
                    await _surveyAnswerRepository.DeleteWhereAsync(a => questionIds.Contains(a.QuestionId));
                    await _unitOfWork.SaveChangesAsync();
                }

                _surveyRepository.Delete(entity);
                await _unitOfWork.SaveChangesAsync();

                await _unitOfWork.CommitTransactionAsync();
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<SurveyDetailResponse> GetByIdAsync(long surveyId, bool isHrOrAdmin, long? employeeId)
        {
            var entity = await _surveyRepository.GetByIdAsync(surveyId)
                ?? throw new NotFoundException("Survey", surveyId);

            await EnsureNonHrCallerIsEligibleAsync(entity, isHrOrAdmin, employeeId);

            var isEligible = await IsEligibleForSurveyAsync(entity, employeeId);
            return entity.ToResponse(isEligible);
        }

        public async Task<PagedResult<SurveyDetailResponse>> GetPaginatedAsync(PagedRequest request, long? employeeId)
        {
            var pagedResult = await _surveyRepository.GetPaginatedAsync(request);

            var data = new List<SurveyDetailResponse>();
            foreach (var survey in pagedResult.Data)
            {
                var isEligible = await IsEligibleForSurveyAsync(survey, employeeId);
                data.Add(survey.ToResponse(isEligible));
            }

            return new PagedResult<SurveyDetailResponse>
            {
                Data = data,
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }

        /// <summary>
        /// Whether the given employee's own department/branch actually matches this survey's
        /// audience (see GetEligibleEmployeeIdsAsync) - used to decide SurveyDetailResponse.IsEligible,
        /// which the frontend uses to show/hide "Take Survey". Only ever true for a Published survey:
        /// a Draft/Closed survey can't be responded to regardless of audience match (see
        /// SubmitResponseAsync), so there's no point resolving audience membership for one - this also
        /// avoids the extra query for every Draft/Closed row in a large HR "all surveys" list.
        /// </summary>
        private async Task<bool> IsEligibleForSurveyAsync(Survey survey, long? employeeId)
        {
            if (!employeeId.HasValue || survey.Status != "Published") return false;

            var eligibleIds = await GetEligibleEmployeeIdsAsync(survey.SurveyId);
            return eligibleIds.Contains(employeeId.Value);
        }

        /// <summary>
        /// Employee-facing "surveys I can see" list (US: department/branch-targeted surveys must
        /// only be visible to employees in that department/branch). Draft is never included - a
        /// non-HR caller has no legitimate reason to see a survey still being authored, regardless
        /// of its eventual audience. Published and Closed surveys are both included so an eligible
        /// employee can still review one after it closes.
        /// </summary>
        public async Task<List<SurveyDetailResponse>> GetEligibleAsync(long employeeId)
        {
            var candidateSurveys = (await _surveyRepository.FindAsync(s => s.Status != "Draft")).ToList();
            var result = new List<SurveyDetailResponse>();

            foreach (var survey in candidateSurveys)
            {
                var eligibleIds = await GetEligibleEmployeeIdsAsync(survey.SurveyId);
                if (eligibleIds.Contains(employeeId))
                    result.Add(survey.ToResponse(true));
            }

            return result;
        }

        /// <summary>
        /// Shared by GetByIdAsync/GetQuestionsAsync: an HR/Admin caller bypasses entirely (they
        /// manage surveys of any status/audience); a non-HR caller may never see a Draft survey, and
        /// otherwise must be in its audience's eligible-employee set.
        /// </summary>
        private async Task EnsureNonHrCallerIsEligibleAsync(Survey survey, bool isHrOrAdmin, long? employeeId)
        {
            if (isHrOrAdmin)
                return;

            if (survey.Status == "Draft")
                throw new ForbiddenException("You do not have access to this survey.");

            var eligibleIds = await GetEligibleEmployeeIdsAsync(survey.SurveyId);
            if (employeeId is null || !eligibleIds.Contains(employeeId.Value))
                throw new ForbiddenException("You do not have access to this survey.");
        }

        /// <summary>
        /// Questions and audience targeting are structural to a survey - once it's Published (or
        /// Closed), changing them would silently invalidate already-collected responses/results,
        /// so they're locked to Draft-only. Shared by AddQuestionAsync/UpdateQuestionAsync/
        /// DeleteQuestionAsync/AddAudienceAsync.
        /// </summary>
        private static void EnsureDraft(Survey survey)
        {
            if (survey.Status != "Draft")
            {
                throw new FluentValidation.ValidationException(new[]
                {
                    new FluentValidation.Results.ValidationFailure(nameof(survey.Status),
                        "Questions and audience can only be modified while the survey is in Draft status.")
                });
            }
        }

        public async Task<long> AddQuestionAsync(long surveyId, SurveyQuestionCreateRequest request)
        {
            var survey = await _surveyRepository.GetByIdAsync(surveyId)
                ?? throw new NotFoundException("Survey", surveyId);
            EnsureDraft(survey);

            var question = request.ToEntity(surveyId);

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                await _surveyQuestionRepository.AddAsync(question);
                await _unitOfWork.SaveChangesAsync();

                if (request.Options.Count > 0)
                {
                    var options = request.Options.Select(o => o.ToEntity(question.QuestionId)).ToList();
                    await _surveyOptionRepository.AddRangeAsync(options);
                    await _unitOfWork.SaveChangesAsync();
                }

                await _unitOfWork.CommitTransactionAsync();
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }

            return question.QuestionId;
        }

        public async Task UpdateQuestionAsync(long surveyId, long questionId, SurveyQuestionUpdateRequest request)
        {
            var question = await _surveyQuestionRepository.GetByIdAsync(questionId)
                ?? throw new NotFoundException("SurveyQuestion", questionId);

            if (question.SurveyId != surveyId)
                throw new NotFoundException("SurveyQuestion", questionId);

            var survey = await _surveyRepository.GetByIdAsync(surveyId)
                ?? throw new NotFoundException("Survey", surveyId);
            EnsureDraft(survey);

            question.ApplyUpdate(request);
            _surveyQuestionRepository.Update(question);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteQuestionAsync(long surveyId, long questionId)
        {
            var question = await _surveyQuestionRepository.GetByIdAsync(questionId)
                ?? throw new NotFoundException("SurveyQuestion", questionId);

            if (question.SurveyId != surveyId)
                throw new NotFoundException("SurveyQuestion", questionId);

            var survey = await _surveyRepository.GetByIdAsync(surveyId)
                ?? throw new NotFoundException("Survey", surveyId);
            EnsureDraft(survey);

            var hasAnswers = await _surveyAnswerRepository.ExistsForQuestionAsync(questionId);
            if (hasAnswers)
            {
                throw new FluentValidation.ValidationException(new[]
                {
                    new FluentValidation.Results.ValidationFailure(nameof(questionId),
                        "This question already has submitted answers and cannot be deleted.")
                });
            }

            _surveyQuestionRepository.Delete(question);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<SurveyQuestionResponse>> GetQuestionsAsync(long surveyId, bool isHrOrAdmin, long? employeeId)
        {
            var survey = await _surveyRepository.GetByIdAsync(surveyId)
                ?? throw new NotFoundException("Survey", surveyId);
            await EnsureNonHrCallerIsEligibleAsync(survey, isHrOrAdmin, employeeId);

            var questions = await _surveyQuestionRepository.GetBySurveyIdAsync(surveyId);
            if (questions.Count == 0)
                return new List<SurveyQuestionResponse>();

            var questionIds = questions.Select(q => q.QuestionId).ToList();
            var options = await _surveyOptionRepository.GetByQuestionIdsAsync(questionIds);
            var optionsByQuestion = options.GroupBy(o => o.QuestionId).ToDictionary(g => g.Key, g => g.ToList());

            return questions
                .Select(q => q.ToResponse(optionsByQuestion.TryGetValue(q.QuestionId, out var opts) ? opts : null))
                .ToList();
        }

        public async Task<long> AddAudienceAsync(long surveyId, SurveyAudienceCreateRequest request)
        {
            var survey = await _surveyRepository.GetByIdAsync(surveyId)
                ?? throw new NotFoundException("Survey", surveyId);
            EnsureDraft(survey);

            var entity = request.ToEntity(surveyId);
            await _surveyAudienceRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.AudienceId;
        }

        public async Task<List<SurveyAudienceResponse>> GetAudienceAsync(long surveyId)
        {
            var entities = await _surveyAudienceRepository.GetBySurveyIdAsync(surveyId);
            return entities.Select(e => e.ToResponse()).ToList();
        }

        /// <summary>
        /// Cross-referential checks that FluentValidation can't express here (this codebase's
        /// validators are sync-only - no MustAsync usage anywhere - so anything needing repository
        /// access lives here, alongside SubmitResponseAsync's other stateful business rules):
        /// every answer's QuestionId must belong to this survey, every OptionId must belong to its
        /// QuestionId, no question may be answered twice in one submission, and every IsRequired
        /// question must be answered. All failures are collected and thrown together so the client
        /// gets the full picture in one round trip.
        /// </summary>
        private async Task ValidateAnswersAsync(long surveyId, List<SurveyAnswerSubmitRequest> answers)
        {
            var questions = await _surveyQuestionRepository.GetBySurveyIdAsync(surveyId);
            var questionsById = questions.ToDictionary(q => q.QuestionId);

            var options = questionsById.Count > 0
                ? await _surveyOptionRepository.GetByQuestionIdsAsync(questionsById.Keys)
                : new List<SurveyOption>();
            var optionIdsByQuestion = options
                .GroupBy(o => o.QuestionId)
                .ToDictionary(g => g.Key, g => g.Select(o => o.OptionId).ToHashSet());

            var failures = new List<FluentValidation.Results.ValidationFailure>();

            foreach (var answer in answers)
            {
                if (!questionsById.TryGetValue(answer.QuestionId, out var question))
                {
                    failures.Add(new FluentValidation.Results.ValidationFailure(nameof(answer.QuestionId),
                        $"Question {answer.QuestionId} does not belong to this survey."));
                    continue;
                }

                if (answer.OptionId.HasValue &&
                    (!optionIdsByQuestion.TryGetValue(answer.QuestionId, out var validOptionIds) ||
                     !validOptionIds.Contains(answer.OptionId.Value)))
                {
                    failures.Add(new FluentValidation.Results.ValidationFailure(nameof(answer.OptionId),
                        $"Option {answer.OptionId} does not belong to question {answer.QuestionId}."));
                }

                // Each question type is answered through a specific field on SurveyAnswerSubmitRequest -
                // OptionId for choice types, AnswerText for Text, RatingValue for Rating. This catches a
                // mismatch (e.g. a Rating question answered with AnswerText) that FluentValidation can't
                // express since it has no visibility into QuestionType.
                var hasExpectedAnswerField = question.QuestionType switch
                {
                    SurveyQuestionTypes.Rating => answer.RatingValue.HasValue,
                    SurveyQuestionTypes.Text => !string.IsNullOrWhiteSpace(answer.AnswerText),
                    SurveyQuestionTypes.SingleChoice or SurveyQuestionTypes.MultipleChoice => answer.OptionId.HasValue,
                    _ => true
                };
                if (!hasExpectedAnswerField)
                {
                    failures.Add(new FluentValidation.Results.ValidationFailure(nameof(answer.QuestionId),
                        $"Question {answer.QuestionId} is a {question.QuestionType} question and was not answered with the expected answer type."));
                }
            }

            var duplicateQuestionIds = answers
                .GroupBy(a => a.QuestionId)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key);
            foreach (var questionId in duplicateQuestionIds)
            {
                failures.Add(new FluentValidation.Results.ValidationFailure(nameof(SurveyAnswerSubmitRequest.QuestionId),
                    $"Question {questionId} was answered more than once in this submission."));
            }

            var answeredQuestionIds = answers.Select(a => a.QuestionId).ToHashSet();
            var missingRequired = questions.Where(q => q.IsRequired && !answeredQuestionIds.Contains(q.QuestionId));
            foreach (var question in missingRequired)
            {
                failures.Add(new FluentValidation.Results.ValidationFailure(nameof(SurveyQuestion.IsRequired),
                    $"Question {question.QuestionId} is required and was not answered."));
            }

            if (failures.Count > 0)
                throw new FluentValidation.ValidationException(failures);
        }

        public async Task<long> SubmitResponseAsync(long surveyId, SurveySubmissionRequest request, long employeeId)
        {
            var survey = await _surveyRepository.GetByIdAsync(surveyId)
                ?? throw new NotFoundException("Survey", surveyId);

            if (survey.Status != "Published")
            {
                throw new FluentValidation.ValidationException(new[]
                {
                    new FluentValidation.Results.ValidationFailure(nameof(survey.Status),
                        "Responses can only be submitted to a Published survey.")
                });
            }

            var eligibleIds = await GetEligibleEmployeeIdsAsync(surveyId);
            if (!eligibleIds.Contains(employeeId))
                throw new ForbiddenException("You are not eligible to respond to this survey.");

            var alreadyResponded = await _surveyResponseRepository.ExistsAsync(surveyId, employeeId);
            if (alreadyResponded)
                throw new DuplicateException("SurveyResponse", "EmployeeId", employeeId.ToString());

            await ValidateAnswersAsync(surveyId, request.Answers);

            var response = request.ToEntity(surveyId, employeeId);

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                await _surveyResponseRepository.AddAsync(response);
                await _unitOfWork.SaveChangesAsync();

                if (request.Answers.Count > 0)
                {
                    var answers = request.Answers.Select(a => a.ToEntity(response.ResponseId)).ToList();
                    await _surveyAnswerRepository.AddRangeAsync(answers);
                    await _unitOfWork.SaveChangesAsync();
                }

                await _unitOfWork.CommitTransactionAsync();
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }

            return response.ResponseId;
        }

        public async Task<PagedResult<SurveySubmissionResponse>> GetResponsesAsync(long surveyId, PagedRequest request)
        {
            var survey = await _surveyRepository.GetByIdAsync(surveyId)
                ?? throw new NotFoundException("Survey", surveyId);

            var pagedResult = await _surveyResponseRepository.GetPaginatedBySurveyIdAsync(surveyId, request);

            var responseIds = pagedResult.Data.Select(r => r.ResponseId).ToList();
            var answers = responseIds.Count > 0
                ? await _surveyAnswerRepository.GetByResponseIdsAsync(responseIds)
                : new List<SurveyAnswer>();
            var answersByResponse = answers.GroupBy(a => a.ResponseId).ToDictionary(g => g.Key, g => g.ToList());

            // SurveyResponse doesn't carry the respondent's name - resolve it from Employee, deduped
            // per distinct EmployeeId on this page (same batch-lookup shape as
            // ChatMessageService.BuildAttachmentGalleryAsync's sendersById). Skipped for anonymous
            // surveys, where the mapper never exposes the name anyway.
            var namesByEmployeeId = new Dictionary<long, string?>();
            if (!survey.IsAnonymous)
            {
                var employeeIds = pagedResult.Data.Select(r => r.EmployeeId).Distinct();
                foreach (var employeeId in employeeIds)
                {
                    var employee = await _employeeRepository.GetByIdAsync(employeeId);
                    namesByEmployeeId[employeeId] = employee?.EmployeeName;
                }
            }

            return new PagedResult<SurveySubmissionResponse>
            {
                Data = pagedResult.Data
                    .Select(r => r.ToResponse(
                        survey.IsAnonymous,
                        namesByEmployeeId.TryGetValue(r.EmployeeId, out var name) ? name : null,
                        answersByResponse.TryGetValue(r.ResponseId, out var ans) ? ans : null))
                    .ToList(),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }

        public async Task<SurveyResultsResponse> GetResultsAsync(long surveyId)
        {
            _ = await _surveyRepository.GetByIdAsync(surveyId)
                ?? throw new NotFoundException("Survey", surveyId);

            var responses = await _surveyResponseRepository.GetAllBySurveyIdAsync(surveyId);
            var totalResponses = responses.Count;

            var responseIds = responses.Select(r => r.ResponseId).ToList();
            var answers = responseIds.Count > 0
                ? await _surveyAnswerRepository.GetByResponseIdsAsync(responseIds)
                : new List<SurveyAnswer>();
            var answersByQuestion = answers.GroupBy(a => a.QuestionId).ToDictionary(g => g.Key, g => g.ToList());

            var questions = await _surveyQuestionRepository.GetBySurveyIdAsync(surveyId);
            var questionIds = questions.Select(q => q.QuestionId).ToList();
            var options = questionIds.Count > 0
                ? await _surveyOptionRepository.GetByQuestionIdsAsync(questionIds)
                : new List<SurveyOption>();
            var optionsByQuestion = options.GroupBy(o => o.QuestionId).ToDictionary(g => g.Key, g => g.ToList());

            var questionResults = questions.Select(q =>
            {
                var qAnswers = answersByQuestion.TryGetValue(q.QuestionId, out var ans) ? ans : new List<SurveyAnswer>();
                var qOptions = optionsByQuestion.TryGetValue(q.QuestionId, out var opts) ? opts : new List<SurveyOption>();

                // Denominator is the number of distinct responses that actually answered THIS
                // question, not the survey's total response count - a question isn't guaranteed to
                // be answered by every respondent (it may have been added after some responses were
                // already submitted, or simply be optional), so dividing by totalResponses would
                // understate its option percentages whenever that happens.
                var questionRespondentCount = qAnswers.Select(a => a.ResponseId).Distinct().Count();

                var optionResults = qOptions.Select(o =>
                {
                    var count = qAnswers.Count(a => a.OptionId == o.OptionId);
                    return new SurveyOptionResultResponse
                    {
                        OptionId = o.OptionId,
                        OptionText = o.OptionText,
                        Count = count,
                        Percentage = questionRespondentCount > 0 ? Math.Round(100m * count / questionRespondentCount, 1) : 0m
                    };
                }).ToList();

                var textAnswers = qAnswers
                    .Where(a => a.OptionId == null && a.RatingValue == null && !string.IsNullOrWhiteSpace(a.AnswerText))
                    .Select(a => a.AnswerText!)
                    .ToList();

                SurveyRatingResultResponse? rating = null;
                if (q.QuestionType == SurveyQuestionTypes.Rating)
                {
                    var ratingValues = qAnswers.Where(a => a.RatingValue.HasValue).Select(a => a.RatingValue!.Value).ToList();
                    rating = new SurveyRatingResultResponse
                    {
                        AverageValue = ratingValues.Count > 0 ? Math.Round((decimal)ratingValues.Average(), 2) : 0m,
                        Distribution = ratingValues
                            .GroupBy(v => v)
                            .ToDictionary(g => g.Key, g => g.Count())
                    };
                }

                return new SurveyQuestionResultResponse
                {
                    QuestionId = q.QuestionId,
                    QuestionText = q.QuestionText,
                    QuestionType = q.QuestionType,
                    Options = optionResults,
                    TextAnswers = textAnswers,
                    Rating = rating
                };
            }).ToList();

            decimal? participationRate = null;
            var audience = await _surveyAudienceRepository.GetBySurveyIdAsync(surveyId);
            if (audience.Any(a => a.AudienceType == SurveyAudienceTypes.EntireCompany))
            {
                var totalEmployees = await _employeeRepository.CountActiveAsync();
                participationRate = totalEmployees > 0 ? Math.Round(100m * totalResponses / totalEmployees, 1) : 0m;
            }
            else if (audience.Count > 0)
            {
                var departmentIds = audience
                    .Where(a => a.AudienceType == SurveyAudienceTypes.Department && a.DepartmentId.HasValue)
                    .Select(a => a.DepartmentId!.Value);
                var siteIds = audience
                    .Where(a => a.AudienceType == SurveyAudienceTypes.Branch && a.BranchId.HasValue)
                    .Select(a => a.BranchId!.Value);

                var eligibleEmployees = await _employeeRepository.CountActiveByDepartmentOrSiteIdsAsync(departmentIds, siteIds);
                participationRate = eligibleEmployees > 0 ? Math.Round(100m * totalResponses / eligibleEmployees, 1) : 0m;
            }

            return new SurveyResultsResponse
            {
                SurveyId = surveyId,
                TotalResponses = totalResponses,
                ParticipationRate = participationRate,
                Questions = questionResults
            };
        }
    }
}
