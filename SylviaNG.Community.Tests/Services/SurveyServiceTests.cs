using FluentAssertions;
using Moq;
using SylviaNG.Community.Application.Common.Exceptions;
using SylviaNG.Community.Application.Features.Surveys.Models;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Application.Services;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Tests.Services;

public class SurveyServiceTests
{
    private readonly Mock<ISurveyRepository> _surveyRepositoryMock;
    private readonly Mock<ISurveyAudienceRepository> _surveyAudienceRepositoryMock;
    private readonly Mock<ISurveyQuestionRepository> _surveyQuestionRepositoryMock;
    private readonly Mock<ISurveyOptionRepository> _surveyOptionRepositoryMock;
    private readonly Mock<ISurveyResponseRepository> _surveyResponseRepositoryMock;
    private readonly Mock<ISurveyAnswerRepository> _surveyAnswerRepositoryMock;
    private readonly Mock<IEmployeeRepository> _employeeRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly SurveyService _service;

    public SurveyServiceTests()
    {
        _surveyRepositoryMock = new Mock<ISurveyRepository>();
        _surveyAudienceRepositoryMock = new Mock<ISurveyAudienceRepository>();
        _surveyQuestionRepositoryMock = new Mock<ISurveyQuestionRepository>();
        _surveyOptionRepositoryMock = new Mock<ISurveyOptionRepository>();
        _surveyResponseRepositoryMock = new Mock<ISurveyResponseRepository>();
        _surveyAnswerRepositoryMock = new Mock<ISurveyAnswerRepository>();
        _employeeRepositoryMock = new Mock<IEmployeeRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _service = new SurveyService(
            _surveyRepositoryMock.Object,
            _surveyAudienceRepositoryMock.Object,
            _surveyQuestionRepositoryMock.Object,
            _surveyOptionRepositoryMock.Object,
            _surveyResponseRepositoryMock.Object,
            _surveyAnswerRepositoryMock.Object,
            _employeeRepositoryMock.Object,
            _unitOfWorkMock.Object);
    }

    #region Survey CRUD

    [Fact]
    public async System.Threading.Tasks.Task CreateAsync_WithValidRequest_ShouldReturnId()
    {
        var request = new SurveyCreateRequest { Title = "Engagement Pulse", SurveyType = "Pulse" };

        _surveyRepositoryMock.Setup(r => r.ExistsByTitleAsync(request.Title, null)).ReturnsAsync(false);
        _surveyRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Survey>()))
            .Callback<Survey>(s => s.SurveyId = 1);

        var result = await _service.CreateAsync(request);

        result.Should().Be(1);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task CreateAsync_WithExternalUrl_ShouldPersistExternalUrl()
    {
        var request = new SurveyCreateRequest { Title = "External Pulse", SurveyType = "Pulse", ExternalUrl = "https://forms.google.com/abc123" };
        Survey? added = null;

        _surveyRepositoryMock.Setup(r => r.ExistsByTitleAsync(request.Title, null)).ReturnsAsync(false);
        _surveyRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Survey>()))
            .Callback<Survey>(s => { s.SurveyId = 1; added = s; });

        await _service.CreateAsync(request);

        added!.ExternalUrl.Should().Be("https://forms.google.com/abc123");
    }

    [Fact]
    public async System.Threading.Tasks.Task CreateAsync_WithDuplicateTitle_ShouldThrowDuplicateException()
    {
        var request = new SurveyCreateRequest { Title = "Engagement Pulse", SurveyType = "Pulse" };
        _surveyRepositoryMock.Setup(r => r.ExistsByTitleAsync(request.Title, null)).ReturnsAsync(true);

        var act = () => _service.CreateAsync(request);

        await act.Should().ThrowAsync<DuplicateException>().WithMessage("*Engagement Pulse*");
    }

    [Fact]
    public async System.Threading.Tasks.Task UpdateAsync_WhenNotFound_ShouldThrowNotFoundException()
    {
        _surveyRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Survey?)null);

        var act = () => _service.UpdateAsync(1, new SurveyUpdateRequest { Title = "New" });

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async System.Threading.Tasks.Task UpdateAsync_WithValidRequest_ShouldApplyChangesAndSave()
    {
        var survey = new Survey { SurveyId = 1, Title = "Old", SurveyType = "Pulse", Status = "Draft" };
        _surveyRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(survey);

        await _service.UpdateAsync(1, new SurveyUpdateRequest { Title = "New Title" });

        survey.Title.Should().Be("New Title");
        _surveyRepositoryMock.Verify(r => r.Update(survey), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task UpdateAsync_WithExternalUrl_ShouldApplyExternalUrl()
    {
        var survey = new Survey { SurveyId = 1, Title = "Old", SurveyType = "Pulse", Status = "Draft" };
        _surveyRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(survey);

        await _service.UpdateAsync(1, new SurveyUpdateRequest { ExternalUrl = "https://forms.google.com/xyz789" });

        survey.ExternalUrl.Should().Be("https://forms.google.com/xyz789");
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task UpdateAsync_WithEmptyExternalUrl_ShouldClearExternalUrl()
    {
        var survey = new Survey { SurveyId = 1, Title = "Old", SurveyType = "Pulse", Status = "Draft", ExternalUrl = "https://forms.google.com/old" };
        _surveyRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(survey);

        await _service.UpdateAsync(1, new SurveyUpdateRequest { ExternalUrl = "" });

        survey.ExternalUrl.Should().BeNull();
    }

    [Fact]
    public async System.Threading.Tasks.Task UpdateAsync_WithNullExternalUrl_ShouldLeaveExternalUrlUnchanged()
    {
        var survey = new Survey { SurveyId = 1, Title = "Old", SurveyType = "Pulse", Status = "Draft", ExternalUrl = "https://forms.google.com/old" };
        _surveyRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(survey);

        await _service.UpdateAsync(1, new SurveyUpdateRequest { Title = "New Title" });

        survey.ExternalUrl.Should().Be("https://forms.google.com/old");
    }

    [Fact]
    public async System.Threading.Tasks.Task PublishAsync_WhenNotFound_ShouldThrowNotFoundException()
    {
        _surveyRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Survey?)null);

        var act = () => _service.PublishAsync(1);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async System.Threading.Tasks.Task PublishAsync_ShouldSetStatusToPublishedAndSetPublishedAt()
    {
        var survey = new Survey { SurveyId = 1, Title = "Pulse", SurveyType = "Pulse", Status = "Draft" };
        _surveyRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(survey);

        await _service.PublishAsync(1);

        survey.Status.Should().Be("Published");
        survey.PublishedAt.Should().NotBeNull();
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task CloseAsync_ShouldSetStatusToClosedAndSetClosedAt()
    {
        var survey = new Survey { SurveyId = 1, Title = "Pulse", SurveyType = "Pulse", Status = "Published" };
        _surveyRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(survey);

        await _service.CloseAsync(1);

        survey.Status.Should().Be("Closed");
        survey.ClosedAt.Should().NotBeNull();
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task DeleteAsync_WhenNotFound_ShouldThrowNotFoundException()
    {
        _surveyRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Survey?)null);

        var act = () => _service.DeleteAsync(1);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async System.Threading.Tasks.Task DeleteAsync_WhenClosed_ShouldThrowValidationException()
    {
        var survey = new Survey { SurveyId = 1, Title = "Pulse", SurveyType = "Pulse", Status = "Closed" };
        _surveyRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(survey);

        var act = () => _service.DeleteAsync(1);

        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
        _surveyRepositoryMock.Verify(r => r.Delete(It.IsAny<Survey>()), Times.Never);
    }

    [Fact]
    public async System.Threading.Tasks.Task DeleteAsync_WhenDraft_ShouldDeleteAndSave()
    {
        var survey = new Survey { SurveyId = 1, Title = "Pulse", SurveyType = "Pulse", Status = "Draft" };
        _surveyRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(survey);

        await _service.DeleteAsync(1);

        _surveyRepositoryMock.Verify(r => r.Delete(survey), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task GetByIdAsync_WhenNotFound_ShouldThrowNotFoundException()
    {
        _surveyRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Survey?)null);

        var act = () => _service.GetByIdAsync(1);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async System.Threading.Tasks.Task GetByIdAsync_WhenFound_ShouldReturnMappedResponse()
    {
        var survey = new Survey { SurveyId = 1, Title = "Pulse", SurveyType = "Pulse", Status = "Draft" };
        _surveyRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(survey);

        var result = await _service.GetByIdAsync(1);

        result.SurveyId.Should().Be(1);
        result.Title.Should().Be("Pulse");
    }

    [Fact]
    public async System.Threading.Tasks.Task GetPaginatedAsync_ShouldReturnMappedPagedResult()
    {
        var pagedEntities = new PagedResult<Survey>
        {
            Data = new List<Survey> { new() { SurveyId = 1, Title = "Pulse", SurveyType = "Pulse", Status = "Draft" } },
            TotalCount = 1,
            PageNumber = 1,
            PageSize = 10
        };
        _surveyRepositoryMock.Setup(r => r.GetPaginatedAsync(It.IsAny<PagedRequest>())).ReturnsAsync(pagedEntities);

        var result = await _service.GetPaginatedAsync(new PagedRequest());

        result.TotalCount.Should().Be(1);
        result.Data.Should().ContainSingle(s => s.SurveyId == 1);
    }

    #endregion

    #region Questions & Options

    [Fact]
    public async System.Threading.Tasks.Task AddQuestionAsync_WhenSurveyNotFound_ShouldThrowNotFoundException()
    {
        _surveyRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Survey?)null);

        var act = () => _service.AddQuestionAsync(1, new SurveyQuestionCreateRequest { QuestionText = "Q1", QuestionType = "Text" });

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async System.Threading.Tasks.Task AddQuestionAsync_WithOptions_ShouldAddQuestionAndOptions()
    {
        _surveyRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Survey { SurveyId = 1, Title = "Pulse", SurveyType = "Pulse" });
        _surveyQuestionRepositoryMock.Setup(r => r.AddAsync(It.IsAny<SurveyQuestion>()))
            .Callback<SurveyQuestion>(q => q.QuestionId = 5);

        var request = new SurveyQuestionCreateRequest
        {
            QuestionText = "How satisfied are you?",
            QuestionType = "SingleChoice",
            Options = new List<SurveyOptionCreateRequest>
            {
                new() { OptionText = "Satisfied", DisplayOrder = 1 },
                new() { OptionText = "Unsatisfied", DisplayOrder = 2 }
            }
        };

        var result = await _service.AddQuestionAsync(1, request);

        result.Should().Be(5);
        _surveyOptionRepositoryMock.Verify(r => r.AddRangeAsync(It.Is<IEnumerable<SurveyOption>>(
            opts => opts.Count() == 2 && opts.All(o => o.QuestionId == 5))), Times.Once);
        _unitOfWorkMock.Verify(u => u.BeginTransactionAsync(), Times.Once);
        _unitOfWorkMock.Verify(u => u.CommitTransactionAsync(), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Exactly(2));
    }

    [Fact]
    public async System.Threading.Tasks.Task AddQuestionAsync_WithoutOptions_ShouldNotCallOptionRepository()
    {
        _surveyRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Survey { SurveyId = 1, Title = "Pulse", SurveyType = "Pulse" });
        _surveyQuestionRepositoryMock.Setup(r => r.AddAsync(It.IsAny<SurveyQuestion>()))
            .Callback<SurveyQuestion>(q => q.QuestionId = 7);

        var request = new SurveyQuestionCreateRequest { QuestionText = "Free text", QuestionType = "Text" };

        var result = await _service.AddQuestionAsync(1, request);

        result.Should().Be(7);
        _surveyOptionRepositoryMock.Verify(r => r.AddRangeAsync(It.IsAny<IEnumerable<SurveyOption>>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task UpdateQuestionAsync_WhenQuestionNotFound_ShouldThrowNotFoundException()
    {
        _surveyQuestionRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((SurveyQuestion?)null);

        var act = () => _service.UpdateQuestionAsync(1, 1, new SurveyQuestionUpdateRequest());

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async System.Threading.Tasks.Task UpdateQuestionAsync_WhenQuestionBelongsToDifferentSurvey_ShouldThrowNotFoundException()
    {
        _surveyQuestionRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new SurveyQuestion { QuestionId = 1, SurveyId = 99 });

        var act = () => _service.UpdateQuestionAsync(1, 1, new SurveyQuestionUpdateRequest());

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async System.Threading.Tasks.Task DeleteQuestionAsync_WhenFound_ShouldDeleteAndSave()
    {
        var question = new SurveyQuestion { QuestionId = 1, SurveyId = 1 };
        _surveyQuestionRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(question);

        await _service.DeleteQuestionAsync(1, 1);

        _surveyQuestionRepositoryMock.Verify(r => r.Delete(question), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task GetQuestionsAsync_ShouldReturnQuestionsWithMatchingOptions()
    {
        var questions = new List<SurveyQuestion>
        {
            new() { QuestionId = 1, SurveyId = 1, QuestionText = "Q1", QuestionType = "SingleChoice" }
        };
        var options = new List<SurveyOption>
        {
            new() { OptionId = 10, QuestionId = 1, OptionText = "Yes" },
            new() { OptionId = 11, QuestionId = 1, OptionText = "No" }
        };

        _surveyQuestionRepositoryMock.Setup(r => r.GetBySurveyIdAsync(1)).ReturnsAsync(questions);
        _surveyOptionRepositoryMock.Setup(r => r.GetByQuestionIdsAsync(It.IsAny<IEnumerable<long>>())).ReturnsAsync(options);

        var result = await _service.GetQuestionsAsync(1);

        result.Should().ContainSingle();
        result[0].Options.Should().HaveCount(2);
    }

    #endregion

    #region Audience

    [Fact]
    public async System.Threading.Tasks.Task AddAudienceAsync_WhenSurveyExists_ShouldReturnId()
    {
        _surveyRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Survey { SurveyId = 1, Title = "Pulse", SurveyType = "Pulse" });
        _surveyAudienceRepositoryMock.Setup(r => r.AddAsync(It.IsAny<SurveyAudience>()))
            .Callback<SurveyAudience>(a => a.AudienceId = 3);

        var result = await _service.AddAudienceAsync(1, new SurveyAudienceCreateRequest { AudienceType = "Department", DepartmentId = 2 });

        result.Should().Be(3);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task AddAudienceAsync_WhenSurveyNotFound_ShouldThrowNotFoundException()
    {
        _surveyRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Survey?)null);

        var act = () => _service.AddAudienceAsync(1, new SurveyAudienceCreateRequest { AudienceType = "All" });

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async System.Threading.Tasks.Task GetAudienceAsync_ShouldReturnMappedList()
    {
        _surveyAudienceRepositoryMock.Setup(r => r.GetBySurveyIdAsync(1)).ReturnsAsync(new List<SurveyAudience>
        {
            new() { AudienceId = 1, SurveyId = 1, AudienceType = "All" }
        });

        var result = await _service.GetAudienceAsync(1);

        result.Should().ContainSingle(a => a.AudienceId == 1);
    }

    #endregion

    #region Responses & Answers

    [Fact]
    public async System.Threading.Tasks.Task SubmitResponseAsync_WhenSurveyNotFound_ShouldThrowNotFoundException()
    {
        _surveyRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Survey?)null);

        var act = () => _service.SubmitResponseAsync(1, new SurveySubmissionRequest { EmployeeId = 5 });

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async System.Threading.Tasks.Task SubmitResponseAsync_WhenEmployeeAlreadyResponded_ShouldThrowDuplicateException()
    {
        _surveyRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Survey { SurveyId = 1, Title = "Pulse", SurveyType = "Pulse" });
        _surveyResponseRepositoryMock.Setup(r => r.ExistsAsync(1, 5)).ReturnsAsync(true);

        var act = () => _service.SubmitResponseAsync(1, new SurveySubmissionRequest { EmployeeId = 5 });

        await act.Should().ThrowAsync<DuplicateException>();
    }

    [Fact]
    public async System.Threading.Tasks.Task SubmitResponseAsync_WithValidRequest_ShouldCreateResponseAndAnswersTransactionally()
    {
        _surveyRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Survey { SurveyId = 1, Title = "Pulse", SurveyType = "Pulse" });
        _surveyResponseRepositoryMock.Setup(r => r.ExistsAsync(1, 5)).ReturnsAsync(false);
        _surveyResponseRepositoryMock.Setup(r => r.AddAsync(It.IsAny<SurveyResponse>()))
            .Callback<SurveyResponse>(resp => resp.ResponseId = 20);

        var request = new SurveySubmissionRequest
        {
            EmployeeId = 5,
            Answers = new List<SurveyAnswerSubmitRequest>
            {
                new() { QuestionId = 1, OptionId = 10 },
                new() { QuestionId = 2, AnswerText = "Great" }
            }
        };

        var result = await _service.SubmitResponseAsync(1, request);

        result.Should().Be(20);
        _surveyAnswerRepositoryMock.Verify(r => r.AddRangeAsync(It.Is<IEnumerable<SurveyAnswer>>(
            answers => answers.Count() == 2 && answers.All(a => a.ResponseId == 20))), Times.Once);
        _unitOfWorkMock.Verify(u => u.BeginTransactionAsync(), Times.Once);
        _unitOfWorkMock.Verify(u => u.CommitTransactionAsync(), Times.Once);
        _unitOfWorkMock.Verify(u => u.RollbackTransactionAsync(), Times.Never);
    }

    [Fact]
    public async System.Threading.Tasks.Task GetResponsesAsync_ShouldReturnPagedResultWithAnswers()
    {
        var pagedResponses = new PagedResult<SurveyResponse>
        {
            Data = new List<SurveyResponse>
            {
                new() { ResponseId = 20, SurveyId = 1, EmployeeId = 5, CompletionStatus = "Completed" }
            },
            TotalCount = 1,
            PageNumber = 1,
            PageSize = 10
        };
        _surveyResponseRepositoryMock.Setup(r => r.GetPaginatedBySurveyIdAsync(1, It.IsAny<PagedRequest>())).ReturnsAsync(pagedResponses);
        _surveyAnswerRepositoryMock.Setup(r => r.GetByResponseIdsAsync(It.IsAny<IEnumerable<long>>())).ReturnsAsync(new List<SurveyAnswer>
        {
            new() { AnswerId = 1, ResponseId = 20, QuestionId = 1, OptionId = 10 }
        });

        var result = await _service.GetResponsesAsync(1, new PagedRequest());

        result.TotalCount.Should().Be(1);
        result.Data.Should().ContainSingle();
        result.Data[0].Answers.Should().ContainSingle();
    }

    #endregion

    #region Results

    [Fact]
    public async System.Threading.Tasks.Task GetResultsAsync_WhenSurveyNotFound_ShouldThrowNotFoundException()
    {
        _surveyRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Survey?)null);

        var act = () => _service.GetResultsAsync(1);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async System.Threading.Tasks.Task GetResultsAsync_WithDepartmentScopedAudience_ShouldLeaveParticipationRateNull()
    {
        _surveyRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Survey { SurveyId = 1, Title = "Pulse", SurveyType = "Pulse" });

        _surveyResponseRepositoryMock.Setup(r => r.GetAllBySurveyIdAsync(1)).ReturnsAsync(new List<SurveyResponse>
        {
            new() { ResponseId = 20, SurveyId = 1, EmployeeId = 5 },
            new() { ResponseId = 21, SurveyId = 1, EmployeeId = 6 }
        });
        _surveyAnswerRepositoryMock.Setup(r => r.GetByResponseIdsAsync(It.IsAny<IEnumerable<long>>())).ReturnsAsync(new List<SurveyAnswer>
        {
            new() { AnswerId = 1, ResponseId = 20, QuestionId = 1, OptionId = 10 },
            new() { AnswerId = 2, ResponseId = 21, QuestionId = 1, OptionId = 11 },
            new() { AnswerId = 3, ResponseId = 20, QuestionId = 2, AnswerText = "Great" }
        });
        _surveyQuestionRepositoryMock.Setup(r => r.GetBySurveyIdAsync(1)).ReturnsAsync(new List<SurveyQuestion>
        {
            new() { QuestionId = 1, SurveyId = 1, QuestionText = "Favorite color?", QuestionType = "SingleChoice" },
            new() { QuestionId = 2, SurveyId = 1, QuestionText = "Any feedback?", QuestionType = "Text" }
        });
        _surveyOptionRepositoryMock.Setup(r => r.GetByQuestionIdsAsync(It.IsAny<IEnumerable<long>>())).ReturnsAsync(new List<SurveyOption>
        {
            new() { OptionId = 10, QuestionId = 1, OptionText = "Red" },
            new() { OptionId = 11, QuestionId = 1, OptionText = "Blue" }
        });
        _surveyAudienceRepositoryMock.Setup(r => r.GetBySurveyIdAsync(1)).ReturnsAsync(new List<SurveyAudience>
        {
            new() { AudienceId = 1, SurveyId = 1, AudienceType = "Department", DepartmentId = 2 }
        });

        var result = await _service.GetResultsAsync(1);

        result.TotalResponses.Should().Be(2);
        result.ParticipationRate.Should().BeNull();
        _employeeRepositoryMock.Verify(r => r.CountActiveAsync(), Times.Never);

        var choiceQuestion = result.Questions.Should().Contain(q => q.QuestionId == 1).Subject;
        choiceQuestion.Options.Should().HaveCount(2);
        choiceQuestion.Options.Single(o => o.OptionId == 10).Count.Should().Be(1);
        choiceQuestion.Options.Single(o => o.OptionId == 10).Percentage.Should().Be(50m);
        choiceQuestion.Options.Single(o => o.OptionId == 11).Percentage.Should().Be(50m);

        var textQuestion = result.Questions.Should().Contain(q => q.QuestionId == 2).Subject;
        textQuestion.TextAnswers.Should().ContainSingle().Which.Should().Be("Great");
    }

    [Fact]
    public async System.Threading.Tasks.Task GetResultsAsync_WithEntireCompanyAudience_ShouldComputeParticipationRate()
    {
        _surveyRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Survey { SurveyId = 1, Title = "Pulse", SurveyType = "Pulse" });
        _surveyResponseRepositoryMock.Setup(r => r.GetAllBySurveyIdAsync(1)).ReturnsAsync(new List<SurveyResponse>
        {
            new() { ResponseId = 20, SurveyId = 1, EmployeeId = 5 }
        });
        _surveyAnswerRepositoryMock.Setup(r => r.GetByResponseIdsAsync(It.IsAny<IEnumerable<long>>())).ReturnsAsync(new List<SurveyAnswer>());
        _surveyQuestionRepositoryMock.Setup(r => r.GetBySurveyIdAsync(1)).ReturnsAsync(new List<SurveyQuestion>());
        _surveyAudienceRepositoryMock.Setup(r => r.GetBySurveyIdAsync(1)).ReturnsAsync(new List<SurveyAudience>
        {
            new() { AudienceId = 1, SurveyId = 1, AudienceType = "EntireCompany" }
        });
        _employeeRepositoryMock.Setup(r => r.CountActiveAsync()).ReturnsAsync(10);

        var result = await _service.GetResultsAsync(1);

        result.TotalResponses.Should().Be(1);
        result.ParticipationRate.Should().Be(10m);
    }

    #endregion
}
