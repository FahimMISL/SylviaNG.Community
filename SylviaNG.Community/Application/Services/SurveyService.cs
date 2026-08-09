using SylviaNG.Community.Application.Common.Exceptions;
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
        private readonly IUnitOfWork _unitOfWork;

        public SurveyService(
            ISurveyRepository surveyRepository,
            ISurveyAudienceRepository surveyAudienceRepository,
            ISurveyQuestionRepository surveyQuestionRepository,
            ISurveyOptionRepository surveyOptionRepository,
            ISurveyResponseRepository surveyResponseRepository,
            ISurveyAnswerRepository surveyAnswerRepository,
            IEmployeeRepository employeeRepository,
            IUnitOfWork unitOfWork)
        {
            _surveyRepository = surveyRepository;
            _surveyAudienceRepository = surveyAudienceRepository;
            _surveyQuestionRepository = surveyQuestionRepository;
            _surveyOptionRepository = surveyOptionRepository;
            _surveyResponseRepository = surveyResponseRepository;
            _surveyAnswerRepository = surveyAnswerRepository;
            _employeeRepository = employeeRepository;
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

            entity.ApplyUpdate(request);
            _surveyRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task PublishAsync(long surveyId)
        {
            var entity = await _surveyRepository.GetByIdAsync(surveyId)
                ?? throw new NotFoundException("Survey", surveyId);

            entity.Status = "Published";
            entity.PublishedAt = DateTime.UtcNow;
            _surveyRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task CloseAsync(long surveyId)
        {
            var entity = await _surveyRepository.GetByIdAsync(surveyId)
                ?? throw new NotFoundException("Survey", surveyId);

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

            _surveyRepository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<SurveyDetailResponse> GetByIdAsync(long surveyId)
        {
            var entity = await _surveyRepository.GetByIdAsync(surveyId)
                ?? throw new NotFoundException("Survey", surveyId);

            return entity.ToResponse();
        }

        public async Task<PagedResult<SurveyDetailResponse>> GetPaginatedAsync(PagedRequest request)
        {
            var pagedResult = await _surveyRepository.GetPaginatedAsync(request);

            return new PagedResult<SurveyDetailResponse>
            {
                Data = pagedResult.Data.Select(e => e.ToResponse()).ToList(),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }

        public async Task<long> AddQuestionAsync(long surveyId, SurveyQuestionCreateRequest request)
        {
            _ = await _surveyRepository.GetByIdAsync(surveyId)
                ?? throw new NotFoundException("Survey", surveyId);

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

            _surveyQuestionRepository.Delete(question);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<SurveyQuestionResponse>> GetQuestionsAsync(long surveyId)
        {
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
            _ = await _surveyRepository.GetByIdAsync(surveyId)
                ?? throw new NotFoundException("Survey", surveyId);

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

        public async Task<long> SubmitResponseAsync(long surveyId, SurveySubmissionRequest request)
        {
            _ = await _surveyRepository.GetByIdAsync(surveyId)
                ?? throw new NotFoundException("Survey", surveyId);

            var alreadyResponded = await _surveyResponseRepository.ExistsAsync(surveyId, request.EmployeeId);
            if (alreadyResponded)
                throw new DuplicateException("SurveyResponse", "EmployeeId", request.EmployeeId.ToString());

            var response = request.ToEntity(surveyId);

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
            var pagedResult = await _surveyResponseRepository.GetPaginatedBySurveyIdAsync(surveyId, request);

            var responseIds = pagedResult.Data.Select(r => r.ResponseId).ToList();
            var answers = responseIds.Count > 0
                ? await _surveyAnswerRepository.GetByResponseIdsAsync(responseIds)
                : new List<SurveyAnswer>();
            var answersByResponse = answers.GroupBy(a => a.ResponseId).ToDictionary(g => g.Key, g => g.ToList());

            return new PagedResult<SurveySubmissionResponse>
            {
                Data = pagedResult.Data
                    .Select(r => r.ToResponse(answersByResponse.TryGetValue(r.ResponseId, out var ans) ? ans : null))
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

                var optionResults = qOptions.Select(o =>
                {
                    var count = qAnswers.Count(a => a.OptionId == o.OptionId);
                    return new SurveyOptionResultResponse
                    {
                        OptionId = o.OptionId,
                        OptionText = o.OptionText,
                        Count = count,
                        Percentage = totalResponses > 0 ? Math.Round(100m * count / totalResponses, 1) : 0m
                    };
                }).ToList();

                var textAnswers = qAnswers
                    .Where(a => a.OptionId == null && !string.IsNullOrWhiteSpace(a.AnswerText))
                    .Select(a => a.AnswerText!)
                    .ToList();

                return new SurveyQuestionResultResponse
                {
                    QuestionId = q.QuestionId,
                    QuestionText = q.QuestionText,
                    QuestionType = q.QuestionType,
                    Options = optionResults,
                    TextAnswers = textAnswers
                };
            }).ToList();

            decimal? participationRate = null;
            var audience = await _surveyAudienceRepository.GetBySurveyIdAsync(surveyId);
            if (audience.Any(a => a.AudienceType == SurveyAudienceTypes.EntireCompany))
            {
                var totalEmployees = await _employeeRepository.CountActiveAsync();
                participationRate = totalEmployees > 0 ? Math.Round(100m * totalResponses / totalEmployees, 1) : 0m;
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
