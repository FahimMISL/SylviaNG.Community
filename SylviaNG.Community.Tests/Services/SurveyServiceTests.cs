using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SylviaNG.Community.Application.Common.Exceptions;
using SylviaNG.Community.Application.Features.Notifications.Models;
using SylviaNG.Community.Application.Features.Surveys.Models;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.Application.Services;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;
using SylviaNG.Community.SharedKernel.Pagination;
using System.Linq.Expressions;

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
    private readonly Mock<INotificationService> _notificationServiceMock;
    private readonly Mock<ILogger<SurveyService>> _loggerMock;
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
        _notificationServiceMock = new Mock<INotificationService>();
        _loggerMock = new Mock<ILogger<SurveyService>>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _service = new SurveyService(
            _surveyRepositoryMock.Object,
            _surveyAudienceRepositoryMock.Object,
            _surveyQuestionRepositoryMock.Object,
            _surveyOptionRepositoryMock.Object,
            _surveyResponseRepositoryMock.Object,
            _surveyAnswerRepositoryMock.Object,
            _employeeRepositoryMock.Object,
            _notificationServiceMock.Object,
            _loggerMock.Object,
            _unitOfWorkMock.Object);

        // Default: no audience configured, and employee 5 (the id used throughout
        // SubmitResponseAsync_*/GetByIdAsync_*/GetQuestionsAsync_* tests below) counts as an active
        // employee - "no audience rows" resolves to "everyone active" (see
        // SurveyService.GetEligibleEmployeeIdsAsync), so this keeps the many existing tests that
        // never touch audience/employee mocks passing under the new SubmitResponseAsync/GetByIdAsync/
        // GetQuestionsAsync eligibility checks, while the PublishAsync_* tests below that only assert
        // status/SaveChanges (not notification counts) are unaffected by employee 5 now resolving to
        // one notification. Per-test setups (e.g. GetResultsAsync_* configuring GetBySurveyIdAsync(1),
        // or the eligibility tests configuring a Department/Branch audience) still win via Moq's
        // most-recent-setup-wins resolution.
        _surveyAudienceRepositoryMock.Setup(r => r.GetBySurveyIdAsync(It.IsAny<long>()))
            .ReturnsAsync(new List<SurveyAudience>());
        _employeeRepositoryMock.Setup(r => r.GetActiveIdsAsync())
            .ReturnsAsync(new List<long> { 5 });
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
    public async System.Threading.Tasks.Task UpdateAsync_WhenClosed_ShouldThrowValidationException()
    {
        var survey = new Survey { SurveyId = 1, Title = "Old", SurveyType = "Pulse", Status = "Closed" };
        _surveyRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(survey);

        var act = () => _service.UpdateAsync(1, new SurveyUpdateRequest { Title = "New" });

        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
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
        _surveyQuestionRepositoryMock.Setup(r => r.GetBySurveyIdAsync(1)).ReturnsAsync(new List<SurveyQuestion>
        {
            new() { QuestionId = 1, SurveyId = 1, QuestionText = "Q1", QuestionType = "Text" }
        });

        await _service.PublishAsync(1);

        survey.Status.Should().Be("Published");
        survey.PublishedAt.Should().NotBeNull();
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task PublishAsync_WithExternalUrlAndNoQuestions_ShouldSucceed()
    {
        var survey = new Survey { SurveyId = 1, Title = "Pulse", SurveyType = "Pulse", Status = "Draft", ExternalUrl = "https://forms.google.com/abc" };
        _surveyRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(survey);

        await _service.PublishAsync(1);

        survey.Status.Should().Be("Published");
    }

    [Fact]
    public async System.Threading.Tasks.Task PublishAsync_WithNoQuestionsAndNoExternalUrl_ShouldThrowValidationException()
    {
        var survey = new Survey { SurveyId = 1, Title = "Pulse", SurveyType = "Pulse", Status = "Draft" };
        _surveyRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(survey);
        _surveyQuestionRepositoryMock.Setup(r => r.GetBySurveyIdAsync(1)).ReturnsAsync(new List<SurveyQuestion>());

        var act = () => _service.PublishAsync(1);

        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    [Fact]
    public async System.Threading.Tasks.Task PublishAsync_WhenAlreadyPublished_ShouldThrowValidationException()
    {
        var survey = new Survey { SurveyId = 1, Title = "Pulse", SurveyType = "Pulse", Status = "Published" };
        _surveyRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(survey);

        var act = () => _service.PublishAsync(1);

        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    [Fact]
    public async System.Threading.Tasks.Task PublishAsync_WhenClosed_ShouldThrowValidationException()
    {
        var survey = new Survey { SurveyId = 1, Title = "Pulse", SurveyType = "Pulse", Status = "Closed" };
        _surveyRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(survey);

        var act = () => _service.PublishAsync(1);

        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    [Fact]
    public async System.Threading.Tasks.Task PublishAsync_WithNoAudienceConfigured_ShouldNotifyEveryActiveEmployee()
    {
        var survey = new Survey { SurveyId = 1, Title = "Pulse", SurveyType = "Pulse", Status = "Draft" };
        _surveyRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(survey);
        _surveyQuestionRepositoryMock.Setup(r => r.GetBySurveyIdAsync(1)).ReturnsAsync(new List<SurveyQuestion>
        {
            new() { QuestionId = 1, SurveyId = 1, QuestionText = "Q1", QuestionType = "Text" }
        });
        _employeeRepositoryMock.Setup(r => r.GetActiveIdsAsync()).ReturnsAsync(new List<long> { 5, 6, 7 });

        await _service.PublishAsync(1);

        _notificationServiceMock.Verify(n => n.CreateAsync(It.IsAny<NotificationCreateRequest>()), Times.Exactly(3));
    }

    [Fact]
    public async System.Threading.Tasks.Task PublishAsync_WithEntireCompanyAudience_ShouldNotifyEveryActiveEmployee()
    {
        var survey = new Survey { SurveyId = 1, Title = "Pulse", SurveyType = "Pulse", Status = "Draft" };
        _surveyRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(survey);
        _surveyQuestionRepositoryMock.Setup(r => r.GetBySurveyIdAsync(1)).ReturnsAsync(new List<SurveyQuestion>
        {
            new() { QuestionId = 1, SurveyId = 1, QuestionText = "Q1", QuestionType = "Text" }
        });
        _surveyAudienceRepositoryMock.Setup(r => r.GetBySurveyIdAsync(1)).ReturnsAsync(new List<SurveyAudience>
        {
            new() { AudienceId = 1, SurveyId = 1, AudienceType = "EntireCompany" }
        });
        _employeeRepositoryMock.Setup(r => r.GetActiveIdsAsync()).ReturnsAsync(new List<long> { 5, 6 });

        await _service.PublishAsync(1);

        _notificationServiceMock.Verify(n => n.CreateAsync(It.IsAny<NotificationCreateRequest>()), Times.Exactly(2));
        _employeeRepositoryMock.Verify(r => r.GetActiveIdsByDepartmentIdsAsync(It.IsAny<IEnumerable<long>>()), Times.Never);
    }

    [Fact]
    public async System.Threading.Tasks.Task PublishAsync_WithDepartmentAndBranchAudience_ShouldNotifyUnionOfEligibleEmployeesOnce()
    {
        var survey = new Survey { SurveyId = 1, Title = "Pulse", SurveyType = "Pulse", Status = "Draft" };
        _surveyRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(survey);
        _surveyQuestionRepositoryMock.Setup(r => r.GetBySurveyIdAsync(1)).ReturnsAsync(new List<SurveyQuestion>
        {
            new() { QuestionId = 1, SurveyId = 1, QuestionText = "Q1", QuestionType = "Text" }
        });
        _surveyAudienceRepositoryMock.Setup(r => r.GetBySurveyIdAsync(1)).ReturnsAsync(new List<SurveyAudience>
        {
            new() { AudienceId = 1, SurveyId = 1, AudienceType = "Department", DepartmentId = 2 },
            new() { AudienceId = 2, SurveyId = 1, AudienceType = "Branch", BranchId = 3 }
        });
        _employeeRepositoryMock.Setup(r => r.GetActiveIdsByDepartmentIdsAsync(It.Is<IEnumerable<long>>(ids => ids.Contains(2L))))
            .ReturnsAsync(new List<long> { 5, 6 });
        _employeeRepositoryMock.Setup(r => r.GetActiveIdsBySiteIdsAsync(It.Is<IEnumerable<long>>(ids => ids.Contains(3L))))
            .ReturnsAsync(new List<long> { 6, 7 });

        await _service.PublishAsync(1);

        _notificationServiceMock.Verify(n => n.CreateAsync(It.IsAny<NotificationCreateRequest>()), Times.Exactly(3));
        _employeeRepositoryMock.Verify(r => r.GetActiveIdsAsync(), Times.Never);
    }

    [Fact]
    public async System.Threading.Tasks.Task PublishAsync_ShouldSendCorrectNotificationFields()
    {
        var survey = new Survey { SurveyId = 42, Title = "Engagement Pulse", SurveyType = "Pulse", Status = "Draft" };
        _surveyRepositoryMock.Setup(r => r.GetByIdAsync(42)).ReturnsAsync(survey);
        _surveyQuestionRepositoryMock.Setup(r => r.GetBySurveyIdAsync(42)).ReturnsAsync(new List<SurveyQuestion>
        {
            new() { QuestionId = 1, SurveyId = 42, QuestionText = "Q1", QuestionType = "Text" }
        });
        _employeeRepositoryMock.Setup(r => r.GetActiveIdsAsync()).ReturnsAsync(new List<long> { 9 });

        await _service.PublishAsync(42);

        _notificationServiceMock.Verify(n => n.CreateAsync(It.Is<NotificationCreateRequest>(req =>
            req.EmployeeId == 9 &&
            req.Title == "New survey published: Engagement Pulse" &&
            req.Category == "Survey" &&
            req.RelatedEntityType == "Survey" &&
            req.RelatedEntityId == 42)), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task PublishAsync_WhenNotificationCreateThrows_ShouldStillNotifyRemainingEmployeesAndNotThrow()
    {
        var survey = new Survey { SurveyId = 1, Title = "Pulse", SurveyType = "Pulse", Status = "Draft" };
        _surveyRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(survey);
        _surveyQuestionRepositoryMock.Setup(r => r.GetBySurveyIdAsync(1)).ReturnsAsync(new List<SurveyQuestion>
        {
            new() { QuestionId = 1, SurveyId = 1, QuestionText = "Q1", QuestionType = "Text" }
        });
        _employeeRepositoryMock.Setup(r => r.GetActiveIdsAsync()).ReturnsAsync(new List<long> { 5, 6 });
        _notificationServiceMock.Setup(n => n.CreateAsync(It.Is<NotificationCreateRequest>(r => r.EmployeeId == 5)))
            .ThrowsAsync(new InvalidOperationException("db down"));

        var act = () => _service.PublishAsync(1);

        await act.Should().NotThrowAsync();
        _notificationServiceMock.Verify(n => n.CreateAsync(It.Is<NotificationCreateRequest>(r => r.EmployeeId == 6)), Times.Once);
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
    public async System.Threading.Tasks.Task CloseAsync_WhenDraft_ShouldThrowValidationException()
    {
        var survey = new Survey { SurveyId = 1, Title = "Pulse", SurveyType = "Pulse", Status = "Draft" };
        _surveyRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(survey);

        var act = () => _service.CloseAsync(1);

        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    [Fact]
    public async System.Threading.Tasks.Task CloseAsync_WhenAlreadyClosed_ShouldThrowValidationException()
    {
        var survey = new Survey { SurveyId = 1, Title = "Pulse", SurveyType = "Pulse", Status = "Closed" };
        _surveyRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(survey);

        var act = () => _service.CloseAsync(1);

        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
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
        _surveyQuestionRepositoryMock.Setup(r => r.GetBySurveyIdAsync(1)).ReturnsAsync(new List<SurveyQuestion>());

        await _service.DeleteAsync(1);

        _surveyRepositoryMock.Verify(r => r.Delete(survey), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        _surveyAnswerRepositoryMock.Verify(r => r.DeleteWhereAsync(It.IsAny<Expression<Func<SurveyAnswer, bool>>>()), Times.Never);
    }

    [Fact]
    public async System.Threading.Tasks.Task DeleteAsync_WithExistingQuestions_ShouldDeleteAnswersBeforeSurveyWithinTransaction()
    {
        // Regression test: SurveyAnswer.QuestionId is a Restrict FK, so deleting a survey that has
        // questions with recorded answers must explicitly delete those answers first (see the
        // comment in SurveyService.DeleteAsync) - otherwise Postgres rejects the cascade delete
        // with a foreign key violation on FK_SurveyAnswers_SurveyQuestions_QuestionId.
        var survey = new Survey { SurveyId = 1, Title = "Pulse", SurveyType = "Pulse", Status = "Published" };
        _surveyRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(survey);
        _surveyQuestionRepositoryMock.Setup(r => r.GetBySurveyIdAsync(1)).ReturnsAsync(new List<SurveyQuestion>
        {
            new() { QuestionId = 1, SurveyId = 1, QuestionText = "Rate it", QuestionType = "Rating" }
        });

        await _service.DeleteAsync(1);

        _surveyAnswerRepositoryMock.Verify(r => r.DeleteWhereAsync(It.IsAny<Expression<Func<SurveyAnswer, bool>>>()), Times.Once);
        _surveyRepositoryMock.Verify(r => r.Delete(survey), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Exactly(2));
        _unitOfWorkMock.Verify(u => u.BeginTransactionAsync(), Times.Once);
        _unitOfWorkMock.Verify(u => u.CommitTransactionAsync(), Times.Once);
        _unitOfWorkMock.Verify(u => u.RollbackTransactionAsync(), Times.Never);
    }

    [Fact]
    public async System.Threading.Tasks.Task GetByIdAsync_WhenNotFound_ShouldThrowNotFoundException()
    {
        _surveyRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Survey?)null);

        var act = () => _service.GetByIdAsync(1, isHrOrAdmin: true, employeeId: null);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async System.Threading.Tasks.Task GetByIdAsync_WhenFound_ShouldReturnMappedResponse()
    {
        var survey = new Survey { SurveyId = 1, Title = "Pulse", SurveyType = "Pulse", Status = "Draft" };
        _surveyRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(survey);

        var result = await _service.GetByIdAsync(1, isHrOrAdmin: true, employeeId: null);

        result.SurveyId.Should().Be(1);
        result.Title.Should().Be("Pulse");
    }

    [Fact]
    public async System.Threading.Tasks.Task GetByIdAsync_ForHrOrAdminOutsideSurveysAudience_ShouldStillSetIsEligibleFalse()
    {
        // Reproduces "why can't HR take Sur D1": HR can view/manage any survey regardless of
        // audience (isHrOrAdmin bypasses EnsureNonHrCallerIsEligibleAsync below), but IsEligible
        // must still reflect their real department/branch membership, since that's what the
        // frontend uses to decide whether "Take Survey" should show.
        var survey = new Survey { SurveyId = 1, Title = "Dept Pulse", SurveyType = "Pulse", Status = "Published" };
        _surveyRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(survey);
        _surveyAudienceRepositoryMock.Setup(r => r.GetBySurveyIdAsync(1)).ReturnsAsync(new List<SurveyAudience>
        {
            new() { AudienceId = 1, SurveyId = 1, AudienceType = "Department", DepartmentId = 1 }
        });
        _employeeRepositoryMock.Setup(r => r.GetActiveIdsByDepartmentIdsAsync(It.Is<IEnumerable<long>>(ids => ids.Contains(1L))))
            .ReturnsAsync(new List<long> { 99 }); // department 1's employees - doesn't include the HR caller (3)

        var result = await _service.GetByIdAsync(1, isHrOrAdmin: true, employeeId: 3);

        result.IsEligible.Should().BeFalse();
    }

    [Fact]
    public async System.Threading.Tasks.Task GetByIdAsync_WhenNonHrAndDraft_ShouldThrowForbiddenException()
    {
        var survey = new Survey { SurveyId = 1, Title = "Pulse", SurveyType = "Pulse", Status = "Draft" };
        _surveyRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(survey);

        var act = () => _service.GetByIdAsync(1, isHrOrAdmin: false, employeeId: 5);

        await act.Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async System.Threading.Tasks.Task GetByIdAsync_WhenNonHrAndNotEligible_ShouldThrowForbiddenException()
    {
        var survey = new Survey { SurveyId = 1, Title = "Pulse", SurveyType = "Pulse", Status = "Published" };
        _surveyRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(survey);
        _surveyAudienceRepositoryMock.Setup(r => r.GetBySurveyIdAsync(1))
            .ReturnsAsync(new List<SurveyAudience> { new() { AudienceType = "Department", DepartmentId = 10 } });
        _employeeRepositoryMock.Setup(r => r.GetActiveIdsByDepartmentIdsAsync(It.IsAny<IEnumerable<long>>()))
            .ReturnsAsync(new List<long> { 1, 2, 3 });

        var act = () => _service.GetByIdAsync(1, isHrOrAdmin: false, employeeId: 5);

        await act.Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async System.Threading.Tasks.Task GetByIdAsync_WhenNonHrAndEligible_ShouldReturnMappedResponse()
    {
        var survey = new Survey { SurveyId = 1, Title = "Pulse", SurveyType = "Pulse", Status = "Published" };
        _surveyRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(survey);
        _surveyAudienceRepositoryMock.Setup(r => r.GetBySurveyIdAsync(1))
            .ReturnsAsync(new List<SurveyAudience> { new() { AudienceType = "Department", DepartmentId = 10 } });
        _employeeRepositoryMock.Setup(r => r.GetActiveIdsByDepartmentIdsAsync(It.IsAny<IEnumerable<long>>()))
            .ReturnsAsync(new List<long> { 5 });

        var result = await _service.GetByIdAsync(1, isHrOrAdmin: false, employeeId: 5);

        result.SurveyId.Should().Be(1);
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

        var result = await _service.GetPaginatedAsync(new PagedRequest(), employeeId: 5);

        result.TotalCount.Should().Be(1);
        result.Data.Should().ContainSingle(s => s.SurveyId == 1);
        // Draft surveys are never eligible regardless of audience - see IsEligibleForSurveyAsync.
        result.Data.Single().IsEligible.Should().BeFalse();
        _employeeRepositoryMock.Verify(r => r.GetActiveIdsAsync(), Times.Never);
    }

    [Fact]
    public async System.Threading.Tasks.Task GetPaginatedAsync_WithPublishedSurveyMatchingCallersAudience_ShouldSetIsEligibleTrue()
    {
        var pagedEntities = new PagedResult<Survey>
        {
            Data = new List<Survey> { new() { SurveyId = 1, Title = "Pulse", SurveyType = "Pulse", Status = "Published" } },
            TotalCount = 1,
            PageNumber = 1,
            PageSize = 10
        };
        _surveyRepositoryMock.Setup(r => r.GetPaginatedAsync(It.IsAny<PagedRequest>())).ReturnsAsync(pagedEntities);
        _surveyRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(pagedEntities.Data[0]);
        _surveyAudienceRepositoryMock.Setup(r => r.GetBySurveyIdAsync(1)).ReturnsAsync(new List<SurveyAudience>());
        _employeeRepositoryMock.Setup(r => r.GetActiveIdsAsync()).ReturnsAsync(new List<long> { 5 });

        var result = await _service.GetPaginatedAsync(new PagedRequest(), employeeId: 5);

        result.Data.Single().IsEligible.Should().BeTrue();
    }

    [Fact]
    public async System.Threading.Tasks.Task GetPaginatedAsync_WithPublishedSurveyOutsideCallersAudience_ShouldSetIsEligibleFalse()
    {
        var pagedEntities = new PagedResult<Survey>
        {
            Data = new List<Survey> { new() { SurveyId = 1, Title = "Pulse", SurveyType = "Pulse", Status = "Published" } },
            TotalCount = 1,
            PageNumber = 1,
            PageSize = 10
        };
        _surveyRepositoryMock.Setup(r => r.GetPaginatedAsync(It.IsAny<PagedRequest>())).ReturnsAsync(pagedEntities);
        _surveyRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(pagedEntities.Data[0]);
        _surveyAudienceRepositoryMock.Setup(r => r.GetBySurveyIdAsync(1)).ReturnsAsync(new List<SurveyAudience>
        {
            new() { AudienceId = 1, SurveyId = 1, AudienceType = "Department", DepartmentId = 1 }
        });
        _employeeRepositoryMock.Setup(r => r.GetActiveIdsByDepartmentIdsAsync(It.Is<IEnumerable<long>>(ids => ids.Contains(1L))))
            .ReturnsAsync(new List<long> { 99 }); // department 1's employees - doesn't include the caller (5)

        var result = await _service.GetPaginatedAsync(new PagedRequest(), employeeId: 5);

        result.Data.Single().IsEligible.Should().BeFalse();
    }

    [Fact]
    public async System.Threading.Tasks.Task GetEligibleAsync_ShouldExcludeDraftSurveys()
    {
        _surveyRepositoryMock.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Survey, bool>>>()))
            .ReturnsAsync(new List<Survey>());

        var result = await _service.GetEligibleAsync(5);

        result.Should().BeEmpty();
        _surveyRepositoryMock.Verify(r => r.FindAsync(It.IsAny<Expression<Func<Survey, bool>>>()), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task GetEligibleAsync_WhenEmployeeInTargetedDepartment_ShouldIncludeSurvey()
    {
        var survey = new Survey { SurveyId = 1, Title = "Pulse", SurveyType = "Pulse", Status = "Published" };
        _surveyRepositoryMock.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Survey, bool>>>()))
            .ReturnsAsync(new List<Survey> { survey });
        _surveyAudienceRepositoryMock.Setup(r => r.GetBySurveyIdAsync(1))
            .ReturnsAsync(new List<SurveyAudience> { new() { AudienceType = "Department", DepartmentId = 10 } });
        _employeeRepositoryMock.Setup(r => r.GetActiveIdsByDepartmentIdsAsync(It.IsAny<IEnumerable<long>>()))
            .ReturnsAsync(new List<long> { 5 });

        var result = await _service.GetEligibleAsync(5);

        result.Should().ContainSingle(s => s.SurveyId == 1);
    }

    [Fact]
    public async System.Threading.Tasks.Task GetEligibleAsync_WhenEmployeeNotInTargetedDepartment_ShouldExcludeSurvey()
    {
        var survey = new Survey { SurveyId = 1, Title = "Pulse", SurveyType = "Pulse", Status = "Published" };
        _surveyRepositoryMock.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Survey, bool>>>()))
            .ReturnsAsync(new List<Survey> { survey });
        _surveyAudienceRepositoryMock.Setup(r => r.GetBySurveyIdAsync(1))
            .ReturnsAsync(new List<SurveyAudience> { new() { AudienceType = "Department", DepartmentId = 10 } });
        _employeeRepositoryMock.Setup(r => r.GetActiveIdsByDepartmentIdsAsync(It.IsAny<IEnumerable<long>>()))
            .ReturnsAsync(new List<long> { 1, 2, 3 });

        var result = await _service.GetEligibleAsync(5);

        result.Should().BeEmpty();
    }

    [Fact]
    public async System.Threading.Tasks.Task GetEligibleAsync_WhenNoAudienceRows_ShouldIncludeSurveyForEveryone()
    {
        var survey = new Survey { SurveyId = 1, Title = "Pulse", SurveyType = "Pulse", Status = "Closed" };
        _surveyRepositoryMock.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Survey, bool>>>()))
            .ReturnsAsync(new List<Survey> { survey });
        _surveyAudienceRepositoryMock.Setup(r => r.GetBySurveyIdAsync(1))
            .ReturnsAsync(new List<SurveyAudience>());
        _employeeRepositoryMock.Setup(r => r.GetActiveIdsAsync())
            .ReturnsAsync(new List<long> { 5 });

        var result = await _service.GetEligibleAsync(5);

        result.Should().ContainSingle(s => s.SurveyId == 1);
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
    public async System.Threading.Tasks.Task AddQuestionAsync_WhenSurveyNotDraft_ShouldThrowValidationException()
    {
        _surveyRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Survey { SurveyId = 1, Title = "Pulse", SurveyType = "Pulse", Status = "Published" });

        var act = () => _service.AddQuestionAsync(1, new SurveyQuestionCreateRequest { QuestionText = "Q1", QuestionType = "Text" });

        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
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
    public async System.Threading.Tasks.Task UpdateQuestionAsync_WhenSurveyNotDraft_ShouldThrowValidationException()
    {
        _surveyQuestionRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new SurveyQuestion { QuestionId = 1, SurveyId = 1 });
        _surveyRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Survey { SurveyId = 1, Title = "Pulse", SurveyType = "Pulse", Status = "Published" });

        var act = () => _service.UpdateQuestionAsync(1, 1, new SurveyQuestionUpdateRequest());

        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    [Fact]
    public async System.Threading.Tasks.Task DeleteQuestionAsync_WhenFound_ShouldDeleteAndSave()
    {
        var question = new SurveyQuestion { QuestionId = 1, SurveyId = 1 };
        _surveyQuestionRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(question);
        _surveyRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Survey { SurveyId = 1, Title = "Pulse", SurveyType = "Pulse", Status = "Draft" });
        _surveyAnswerRepositoryMock.Setup(r => r.ExistsForQuestionAsync(1)).ReturnsAsync(false);

        await _service.DeleteQuestionAsync(1, 1);

        _surveyQuestionRepositoryMock.Verify(r => r.Delete(question), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task DeleteQuestionAsync_WhenSurveyNotDraft_ShouldThrowValidationException()
    {
        var question = new SurveyQuestion { QuestionId = 1, SurveyId = 1 };
        _surveyQuestionRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(question);
        _surveyRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Survey { SurveyId = 1, Title = "Pulse", SurveyType = "Pulse", Status = "Published" });

        var act = () => _service.DeleteQuestionAsync(1, 1);

        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
        _surveyQuestionRepositoryMock.Verify(r => r.Delete(It.IsAny<SurveyQuestion>()), Times.Never);
    }

    [Fact]
    public async System.Threading.Tasks.Task DeleteQuestionAsync_WhenAnswersExist_ShouldThrowValidationException()
    {
        var question = new SurveyQuestion { QuestionId = 1, SurveyId = 1 };
        _surveyQuestionRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(question);
        _surveyRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Survey { SurveyId = 1, Title = "Pulse", SurveyType = "Pulse", Status = "Draft" });
        _surveyAnswerRepositoryMock.Setup(r => r.ExistsForQuestionAsync(1)).ReturnsAsync(true);

        var act = () => _service.DeleteQuestionAsync(1, 1);

        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
        _surveyQuestionRepositoryMock.Verify(r => r.Delete(It.IsAny<SurveyQuestion>()), Times.Never);
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

        _surveyRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Survey { SurveyId = 1, Title = "Pulse", SurveyType = "Pulse", Status = "Published" });
        _surveyQuestionRepositoryMock.Setup(r => r.GetBySurveyIdAsync(1)).ReturnsAsync(questions);
        _surveyOptionRepositoryMock.Setup(r => r.GetByQuestionIdsAsync(It.IsAny<IEnumerable<long>>())).ReturnsAsync(options);

        var result = await _service.GetQuestionsAsync(1, isHrOrAdmin: true, employeeId: null);

        result.Should().ContainSingle();
        result[0].Options.Should().HaveCount(2);
    }

    [Fact]
    public async System.Threading.Tasks.Task GetQuestionsAsync_WhenSurveyNotFound_ShouldThrowNotFoundException()
    {
        _surveyRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Survey?)null);

        var act = () => _service.GetQuestionsAsync(1, isHrOrAdmin: true, employeeId: null);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async System.Threading.Tasks.Task GetQuestionsAsync_WhenNonHrAndDraft_ShouldThrowForbiddenException()
    {
        _surveyRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Survey { SurveyId = 1, Title = "Pulse", SurveyType = "Pulse", Status = "Draft" });

        var act = () => _service.GetQuestionsAsync(1, isHrOrAdmin: false, employeeId: 5);

        await act.Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async System.Threading.Tasks.Task GetQuestionsAsync_WhenNonHrAndNotEligible_ShouldThrowForbiddenException()
    {
        _surveyRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Survey { SurveyId = 1, Title = "Pulse", SurveyType = "Pulse", Status = "Published" });
        _surveyAudienceRepositoryMock.Setup(r => r.GetBySurveyIdAsync(1))
            .ReturnsAsync(new List<SurveyAudience> { new() { AudienceType = "Branch", BranchId = 20 } });
        _employeeRepositoryMock.Setup(r => r.GetActiveIdsBySiteIdsAsync(It.IsAny<IEnumerable<long>>()))
            .ReturnsAsync(new List<long> { 1, 2, 3 });

        var act = () => _service.GetQuestionsAsync(1, isHrOrAdmin: false, employeeId: 5);

        await act.Should().ThrowAsync<ForbiddenException>();
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
    public async System.Threading.Tasks.Task AddAudienceAsync_WhenSurveyNotDraft_ShouldThrowValidationException()
    {
        _surveyRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Survey { SurveyId = 1, Title = "Pulse", SurveyType = "Pulse", Status = "Published" });

        var act = () => _service.AddAudienceAsync(1, new SurveyAudienceCreateRequest { AudienceType = "Department", DepartmentId = 2 });

        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
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

        var act = () => _service.SubmitResponseAsync(1, new SurveySubmissionRequest(), 5);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async System.Threading.Tasks.Task SubmitResponseAsync_WhenSurveyNotPublished_ShouldThrowValidationException()
    {
        _surveyRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Survey { SurveyId = 1, Title = "Pulse", SurveyType = "Pulse", Status = "Draft" });

        var act = () => _service.SubmitResponseAsync(1, new SurveySubmissionRequest(), 5);

        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    [Fact]
    public async System.Threading.Tasks.Task SubmitResponseAsync_WhenSurveyClosed_ShouldThrowValidationException()
    {
        _surveyRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Survey { SurveyId = 1, Title = "Pulse", SurveyType = "Pulse", Status = "Closed" });

        var act = () => _service.SubmitResponseAsync(1, new SurveySubmissionRequest(), 5);

        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    [Fact]
    public async System.Threading.Tasks.Task SubmitResponseAsync_WhenEmployeeAlreadyResponded_ShouldThrowDuplicateException()
    {
        _surveyRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Survey { SurveyId = 1, Title = "Pulse", SurveyType = "Pulse", Status = "Published" });
        _surveyResponseRepositoryMock.Setup(r => r.ExistsAsync(1, 5)).ReturnsAsync(true);

        var act = () => _service.SubmitResponseAsync(1, new SurveySubmissionRequest(), 5);

        await act.Should().ThrowAsync<DuplicateException>();
    }

    [Fact]
    public async System.Threading.Tasks.Task SubmitResponseAsync_WhenEmployeeNotInTargetedAudience_ShouldThrowForbiddenException()
    {
        _surveyRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Survey { SurveyId = 1, Title = "Pulse", SurveyType = "Pulse", Status = "Published" });
        _surveyAudienceRepositoryMock.Setup(r => r.GetBySurveyIdAsync(1))
            .ReturnsAsync(new List<SurveyAudience> { new() { AudienceType = "Department", DepartmentId = 10 } });
        _employeeRepositoryMock.Setup(r => r.GetActiveIdsByDepartmentIdsAsync(It.IsAny<IEnumerable<long>>()))
            .ReturnsAsync(new List<long> { 1, 2, 3 });

        var act = () => _service.SubmitResponseAsync(1, new SurveySubmissionRequest(), 5);

        await act.Should().ThrowAsync<ForbiddenException>();
        _surveyResponseRepositoryMock.Verify(r => r.AddAsync(It.IsAny<SurveyResponse>()), Times.Never);
    }

    private void SetUpQuestionsForValidation()
    {
        _surveyQuestionRepositoryMock.Setup(r => r.GetBySurveyIdAsync(1)).ReturnsAsync(new List<SurveyQuestion>
        {
            new() { QuestionId = 1, SurveyId = 1, QuestionText = "Pick one", QuestionType = "SingleChoice", IsRequired = false },
            new() { QuestionId = 2, SurveyId = 1, QuestionText = "Any feedback?", QuestionType = "Text", IsRequired = false },
            new() { QuestionId = 3, SurveyId = 1, QuestionText = "Rate your experience", QuestionType = "Rating", IsRequired = false }
        });
        _surveyOptionRepositoryMock.Setup(r => r.GetByQuestionIdsAsync(It.IsAny<IEnumerable<long>>())).ReturnsAsync(new List<SurveyOption>
        {
            new() { OptionId = 10, QuestionId = 1, OptionText = "Yes" },
            new() { OptionId = 11, QuestionId = 1, OptionText = "No" }
        });
    }

    [Fact]
    public async System.Threading.Tasks.Task SubmitResponseAsync_WithValidRequest_ShouldCreateResponseAndAnswersTransactionally()
    {
        _surveyRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Survey { SurveyId = 1, Title = "Pulse", SurveyType = "Pulse", Status = "Published" });
        _surveyResponseRepositoryMock.Setup(r => r.ExistsAsync(1, 5)).ReturnsAsync(false);
        _surveyResponseRepositoryMock.Setup(r => r.AddAsync(It.IsAny<SurveyResponse>()))
            .Callback<SurveyResponse>(resp => resp.ResponseId = 20);
        SetUpQuestionsForValidation();

        var request = new SurveySubmissionRequest
        {
            Answers = new List<SurveyAnswerSubmitRequest>
            {
                new() { QuestionId = 1, OptionId = 10 },
                new() { QuestionId = 2, AnswerText = "Great" }
            }
        };

        var result = await _service.SubmitResponseAsync(1, request, 5);

        result.Should().Be(20);
        _surveyAnswerRepositoryMock.Verify(r => r.AddRangeAsync(It.Is<IEnumerable<SurveyAnswer>>(
            answers => answers.Count() == 2 && answers.All(a => a.ResponseId == 20))), Times.Once);
        _unitOfWorkMock.Verify(u => u.BeginTransactionAsync(), Times.Once);
        _unitOfWorkMock.Verify(u => u.CommitTransactionAsync(), Times.Once);
        _unitOfWorkMock.Verify(u => u.RollbackTransactionAsync(), Times.Never);
    }

    [Fact]
    public async System.Threading.Tasks.Task SubmitResponseAsync_WithQuestionIdNotBelongingToSurvey_ShouldThrowValidationException()
    {
        _surveyRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Survey { SurveyId = 1, Title = "Pulse", SurveyType = "Pulse", Status = "Published" });
        _surveyResponseRepositoryMock.Setup(r => r.ExistsAsync(1, 5)).ReturnsAsync(false);
        SetUpQuestionsForValidation();

        var request = new SurveySubmissionRequest
        {
            Answers = new List<SurveyAnswerSubmitRequest> { new() { QuestionId = 999, AnswerText = "Foreign" } }
        };

        var act = () => _service.SubmitResponseAsync(1, request, 5);

        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    [Fact]
    public async System.Threading.Tasks.Task SubmitResponseAsync_WithOptionIdNotBelongingToQuestion_ShouldThrowValidationException()
    {
        _surveyRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Survey { SurveyId = 1, Title = "Pulse", SurveyType = "Pulse", Status = "Published" });
        _surveyResponseRepositoryMock.Setup(r => r.ExistsAsync(1, 5)).ReturnsAsync(false);
        SetUpQuestionsForValidation();

        // QuestionId 2 is the Text question - OptionId 10 belongs to QuestionId 1, not 2.
        var request = new SurveySubmissionRequest
        {
            Answers = new List<SurveyAnswerSubmitRequest> { new() { QuestionId = 2, OptionId = 10 } }
        };

        var act = () => _service.SubmitResponseAsync(1, request, 5);

        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    [Fact]
    public async System.Threading.Tasks.Task SubmitResponseAsync_WithDuplicateQuestionIdInAnswers_ShouldThrowValidationException()
    {
        _surveyRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Survey { SurveyId = 1, Title = "Pulse", SurveyType = "Pulse", Status = "Published" });
        _surveyResponseRepositoryMock.Setup(r => r.ExistsAsync(1, 5)).ReturnsAsync(false);
        SetUpQuestionsForValidation();

        var request = new SurveySubmissionRequest
        {
            Answers = new List<SurveyAnswerSubmitRequest>
            {
                new() { QuestionId = 1, OptionId = 10 },
                new() { QuestionId = 1, OptionId = 11 }
            }
        };

        var act = () => _service.SubmitResponseAsync(1, request, 5);

        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    [Fact]
    public async System.Threading.Tasks.Task SubmitResponseAsync_MissingRequiredQuestion_ShouldThrowValidationException()
    {
        _surveyRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Survey { SurveyId = 1, Title = "Pulse", SurveyType = "Pulse", Status = "Published" });
        _surveyResponseRepositoryMock.Setup(r => r.ExistsAsync(1, 5)).ReturnsAsync(false);
        _surveyQuestionRepositoryMock.Setup(r => r.GetBySurveyIdAsync(1)).ReturnsAsync(new List<SurveyQuestion>
        {
            new() { QuestionId = 1, SurveyId = 1, QuestionText = "Pick one", QuestionType = "SingleChoice", IsRequired = true }
        });
        _surveyOptionRepositoryMock.Setup(r => r.GetByQuestionIdsAsync(It.IsAny<IEnumerable<long>>())).ReturnsAsync(new List<SurveyOption>
        {
            new() { OptionId = 10, QuestionId = 1, OptionText = "Yes" }
        });

        var request = new SurveySubmissionRequest { Answers = new List<SurveyAnswerSubmitRequest>() };

        var act = () => _service.SubmitResponseAsync(1, request, 5);

        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    [Fact]
    public async System.Threading.Tasks.Task SubmitResponseAsync_WithRatingQuestionAnsweredByRatingValue_ShouldCreateResponseAndAnswersTransactionally()
    {
        _surveyRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Survey { SurveyId = 1, Title = "Pulse", SurveyType = "Pulse", Status = "Published" });
        _surveyResponseRepositoryMock.Setup(r => r.ExistsAsync(1, 5)).ReturnsAsync(false);
        _surveyResponseRepositoryMock.Setup(r => r.AddAsync(It.IsAny<SurveyResponse>()))
            .Callback<SurveyResponse>(resp => resp.ResponseId = 20);
        SetUpQuestionsForValidation();

        // QuestionId 3 is the Rating question from SetUpQuestionsForValidation.
        var request = new SurveySubmissionRequest
        {
            Answers = new List<SurveyAnswerSubmitRequest> { new() { QuestionId = 3, RatingValue = 4 } }
        };

        var result = await _service.SubmitResponseAsync(1, request, 5);

        result.Should().Be(20);
        _surveyAnswerRepositoryMock.Verify(r => r.AddRangeAsync(It.Is<IEnumerable<SurveyAnswer>>(
            answers => answers.Single().RatingValue == 4)), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task SubmitResponseAsync_WithRatingQuestionAnsweredByAnswerText_ShouldThrowValidationException()
    {
        _surveyRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Survey { SurveyId = 1, Title = "Pulse", SurveyType = "Pulse", Status = "Published" });
        _surveyResponseRepositoryMock.Setup(r => r.ExistsAsync(1, 5)).ReturnsAsync(false);
        SetUpQuestionsForValidation();

        // QuestionId 3 is the Rating question - answering it with AnswerText instead of RatingValue
        // is a type mismatch that should be rejected.
        var request = new SurveySubmissionRequest
        {
            Answers = new List<SurveyAnswerSubmitRequest> { new() { QuestionId = 3, AnswerText = "Great" } }
        };

        var act = () => _service.SubmitResponseAsync(1, request, 5);

        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    [Fact]
    public async System.Threading.Tasks.Task GetResponsesAsync_ShouldReturnPagedResultWithAnswersAndEmployeeName()
    {
        _surveyRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Survey { SurveyId = 1, Title = "Pulse", SurveyType = "Pulse", IsAnonymous = false });
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
        _employeeRepositoryMock.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(new Employee { EmployeeId = 5, EmployeeName = "Ayesha Rahman" });

        var result = await _service.GetResponsesAsync(1, new PagedRequest());

        result.TotalCount.Should().Be(1);
        result.Data.Should().ContainSingle();
        result.Data[0].EmployeeId.Should().Be(5);
        result.Data[0].EmployeeName.Should().Be("Ayesha Rahman");
        result.Data[0].Answers.Should().ContainSingle();
    }

    [Fact]
    public async System.Threading.Tasks.Task GetResponsesAsync_WhenSurveyIsAnonymous_ShouldNullOutEmployeeIdAndName()
    {
        _surveyRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Survey { SurveyId = 1, Title = "Pulse", SurveyType = "Pulse", IsAnonymous = true });
        _surveyResponseRepositoryMock.Setup(r => r.GetPaginatedBySurveyIdAsync(1, It.IsAny<PagedRequest>())).ReturnsAsync(new PagedResult<SurveyResponse>
        {
            Data = new List<SurveyResponse> { new() { ResponseId = 20, SurveyId = 1, EmployeeId = 5, CompletionStatus = "Completed" } },
            TotalCount = 1,
            PageNumber = 1,
            PageSize = 10
        });
        _surveyAnswerRepositoryMock.Setup(r => r.GetByResponseIdsAsync(It.IsAny<IEnumerable<long>>())).ReturnsAsync(new List<SurveyAnswer>());
        _employeeRepositoryMock.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(new Employee { EmployeeId = 5, EmployeeName = "Ayesha Rahman" });

        var result = await _service.GetResponsesAsync(1, new PagedRequest());

        var response = result.Data.Should().ContainSingle().Subject;
        response.EmployeeId.Should().BeNull();
        response.EmployeeName.Should().BeNull();
        _employeeRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<long>()), Times.Never);
    }

    [Fact]
    public async System.Threading.Tasks.Task GetResponsesAsync_WhenEmployeeNotFound_ShouldLeaveNameNull()
    {
        _surveyRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Survey { SurveyId = 1, Title = "Pulse", SurveyType = "Pulse", IsAnonymous = false });
        _surveyResponseRepositoryMock.Setup(r => r.GetPaginatedBySurveyIdAsync(1, It.IsAny<PagedRequest>())).ReturnsAsync(new PagedResult<SurveyResponse>
        {
            Data = new List<SurveyResponse> { new() { ResponseId = 20, SurveyId = 1, EmployeeId = 999, CompletionStatus = "Completed" } },
            TotalCount = 1,
            PageNumber = 1,
            PageSize = 10
        });
        _surveyAnswerRepositoryMock.Setup(r => r.GetByResponseIdsAsync(It.IsAny<IEnumerable<long>>())).ReturnsAsync(new List<SurveyAnswer>());
        _employeeRepositoryMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Employee?)null);

        var result = await _service.GetResponsesAsync(1, new PagedRequest());

        result.Data.Should().ContainSingle().Which.EmployeeName.Should().BeNull();
    }

    [Fact]
    public async System.Threading.Tasks.Task GetResponsesAsync_WhenSurveyNotFound_ShouldThrowNotFoundException()
    {
        _surveyRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Survey?)null);

        var act = () => _service.GetResponsesAsync(1, new PagedRequest());

        await act.Should().ThrowAsync<NotFoundException>();
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
    public async System.Threading.Tasks.Task GetResultsAsync_WithDepartmentScopedAudience_ShouldComputeParticipationRateAgainstDepartmentHeadcount()
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
        _employeeRepositoryMock.Setup(r => r.CountActiveByDepartmentOrSiteIdsAsync(It.IsAny<IEnumerable<long>>(), It.IsAny<IEnumerable<long>>()))
            .ReturnsAsync(4);

        var result = await _service.GetResultsAsync(1);

        result.TotalResponses.Should().Be(2);
        result.ParticipationRate.Should().Be(50m);
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
    public async System.Threading.Tasks.Task GetResultsAsync_WithBranchScopedAudience_ShouldComputeParticipationRateAgainstSiteHeadcount()
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
            new() { AudienceId = 1, SurveyId = 1, AudienceType = "Branch", BranchId = 3 }
        });
        _employeeRepositoryMock.Setup(r => r.CountActiveByDepartmentOrSiteIdsAsync(It.IsAny<IEnumerable<long>>(), It.IsAny<IEnumerable<long>>()))
            .ReturnsAsync(5);

        var result = await _service.GetResultsAsync(1);

        result.ParticipationRate.Should().Be(20m);
    }

    [Fact]
    public async System.Threading.Tasks.Task GetResultsAsync_WithMixedDepartmentAndBranchAudience_ShouldDeduplicateOverlappingEmployees()
    {
        _surveyRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Survey { SurveyId = 1, Title = "Pulse", SurveyType = "Pulse" });
        _surveyResponseRepositoryMock.Setup(r => r.GetAllBySurveyIdAsync(1)).ReturnsAsync(new List<SurveyResponse>());
        _surveyAnswerRepositoryMock.Setup(r => r.GetByResponseIdsAsync(It.IsAny<IEnumerable<long>>())).ReturnsAsync(new List<SurveyAnswer>());
        _surveyQuestionRepositoryMock.Setup(r => r.GetBySurveyIdAsync(1)).ReturnsAsync(new List<SurveyQuestion>());
        _surveyAudienceRepositoryMock.Setup(r => r.GetBySurveyIdAsync(1)).ReturnsAsync(new List<SurveyAudience>
        {
            new() { AudienceId = 1, SurveyId = 1, AudienceType = "Department", DepartmentId = 2 },
            new() { AudienceId = 2, SurveyId = 1, AudienceType = "Branch", BranchId = 3 }
        });
        _employeeRepositoryMock
            .Setup(r => r.CountActiveByDepartmentOrSiteIdsAsync(
                It.Is<IEnumerable<long>>(ids => ids.Contains(2L)),
                It.Is<IEnumerable<long>>(ids => ids.Contains(3L))))
            .ReturnsAsync(8);

        var result = await _service.GetResultsAsync(1);

        result.ParticipationRate.Should().Be(0m);
        _employeeRepositoryMock.Verify(r => r.CountActiveByDepartmentOrSiteIdsAsync(
            It.Is<IEnumerable<long>>(ids => ids.Contains(2L)),
            It.Is<IEnumerable<long>>(ids => ids.Contains(3L))), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task GetResultsAsync_WithNoAudienceRows_ShouldLeaveParticipationRateNull()
    {
        _surveyRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Survey { SurveyId = 1, Title = "Pulse", SurveyType = "Pulse" });
        _surveyResponseRepositoryMock.Setup(r => r.GetAllBySurveyIdAsync(1)).ReturnsAsync(new List<SurveyResponse>());
        _surveyAnswerRepositoryMock.Setup(r => r.GetByResponseIdsAsync(It.IsAny<IEnumerable<long>>())).ReturnsAsync(new List<SurveyAnswer>());
        _surveyQuestionRepositoryMock.Setup(r => r.GetBySurveyIdAsync(1)).ReturnsAsync(new List<SurveyQuestion>());
        _surveyAudienceRepositoryMock.Setup(r => r.GetBySurveyIdAsync(1)).ReturnsAsync(new List<SurveyAudience>());

        var result = await _service.GetResultsAsync(1);

        result.ParticipationRate.Should().BeNull();
        _employeeRepositoryMock.Verify(r => r.CountActiveByDepartmentOrSiteIdsAsync(It.IsAny<IEnumerable<long>>(), It.IsAny<IEnumerable<long>>()), Times.Never);
    }

    [Fact]
    public async System.Threading.Tasks.Task GetResultsAsync_WhenSomeRespondentsSkipAnOptionalQuestion_ShouldComputePercentageAgainstQuestionRespondentsNotTotalResponses()
    {
        _surveyRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Survey { SurveyId = 1, Title = "Pulse", SurveyType = "Pulse" });
        _surveyResponseRepositoryMock.Setup(r => r.GetAllBySurveyIdAsync(1)).ReturnsAsync(new List<SurveyResponse>
        {
            new() { ResponseId = 20, SurveyId = 1, EmployeeId = 5 },
            new() { ResponseId = 21, SurveyId = 1, EmployeeId = 6 },
            new() { ResponseId = 22, SurveyId = 1, EmployeeId = 7 }
        });
        // Only responses 20 and 21 answer Q1 (an optional SingleChoice question) - response 22 skips it.
        _surveyAnswerRepositoryMock.Setup(r => r.GetByResponseIdsAsync(It.IsAny<IEnumerable<long>>())).ReturnsAsync(new List<SurveyAnswer>
        {
            new() { AnswerId = 1, ResponseId = 20, QuestionId = 1, OptionId = 10 },
            new() { AnswerId = 2, ResponseId = 21, QuestionId = 1, OptionId = 10 }
        });
        _surveyQuestionRepositoryMock.Setup(r => r.GetBySurveyIdAsync(1)).ReturnsAsync(new List<SurveyQuestion>
        {
            new() { QuestionId = 1, SurveyId = 1, QuestionText = "Favorite color?", QuestionType = "SingleChoice", IsRequired = false }
        });
        _surveyOptionRepositoryMock.Setup(r => r.GetByQuestionIdsAsync(It.IsAny<IEnumerable<long>>())).ReturnsAsync(new List<SurveyOption>
        {
            new() { OptionId = 10, QuestionId = 1, OptionText = "Red" },
            new() { OptionId = 11, QuestionId = 1, OptionText = "Blue" }
        });
        _surveyAudienceRepositoryMock.Setup(r => r.GetBySurveyIdAsync(1)).ReturnsAsync(new List<SurveyAudience>());

        var result = await _service.GetResultsAsync(1);

        result.TotalResponses.Should().Be(3);
        var question = result.Questions.Should().ContainSingle().Subject;
        // Both respondents to this question picked "Red" - percentage should be 100% of the 2
        // respondents who actually answered Q1, not 66.7% of the survey's 3 total responses.
        question.Options.Single(o => o.OptionId == 10).Percentage.Should().Be(100m);
        question.Options.Single(o => o.OptionId == 11).Percentage.Should().Be(0m);
    }

    [Fact]
    public async System.Threading.Tasks.Task GetResultsAsync_WithRatingQuestion_ShouldComputeAverageAndDistribution()
    {
        _surveyRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Survey { SurveyId = 1, Title = "Pulse", SurveyType = "Pulse" });
        _surveyResponseRepositoryMock.Setup(r => r.GetAllBySurveyIdAsync(1)).ReturnsAsync(new List<SurveyResponse>
        {
            new() { ResponseId = 20, SurveyId = 1, EmployeeId = 5 },
            new() { ResponseId = 21, SurveyId = 1, EmployeeId = 6 }
        });
        _surveyAnswerRepositoryMock.Setup(r => r.GetByResponseIdsAsync(It.IsAny<IEnumerable<long>>())).ReturnsAsync(new List<SurveyAnswer>
        {
            new() { AnswerId = 1, ResponseId = 20, QuestionId = 1, RatingValue = 4 },
            new() { AnswerId = 2, ResponseId = 21, QuestionId = 1, RatingValue = 5 }
        });
        _surveyQuestionRepositoryMock.Setup(r => r.GetBySurveyIdAsync(1)).ReturnsAsync(new List<SurveyQuestion>
        {
            new() { QuestionId = 1, SurveyId = 1, QuestionText = "Rate your experience", QuestionType = "Rating" }
        });
        _surveyOptionRepositoryMock.Setup(r => r.GetByQuestionIdsAsync(It.IsAny<IEnumerable<long>>())).ReturnsAsync(new List<SurveyOption>());
        _surveyAudienceRepositoryMock.Setup(r => r.GetBySurveyIdAsync(1)).ReturnsAsync(new List<SurveyAudience>());

        var result = await _service.GetResultsAsync(1);

        var question = result.Questions.Should().ContainSingle().Subject;
        question.Rating.Should().NotBeNull();
        question.Rating!.AverageValue.Should().Be(4.5m);
        question.Rating.Distribution.Should().BeEquivalentTo(new Dictionary<int, int> { { 4, 1 }, { 5, 1 } });
        question.TextAnswers.Should().BeEmpty();
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
