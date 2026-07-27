using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Community.Application.Features.Surveys.Commands.SurveyAudienceAdd;
using SylviaNG.Community.Application.Features.Surveys.Commands.SurveyClose;
using SylviaNG.Community.Application.Features.Surveys.Commands.SurveyCreate;
using SylviaNG.Community.Application.Features.Surveys.Commands.SurveyPublish;
using SylviaNG.Community.Application.Features.Surveys.Commands.SurveyQuestionAdd;
using SylviaNG.Community.Application.Features.Surveys.Commands.SurveyQuestionDelete;
using SylviaNG.Community.Application.Features.Surveys.Commands.SurveyQuestionUpdate;
using SylviaNG.Community.Application.Features.Surveys.Commands.SurveyResponseSubmit;
using SylviaNG.Community.Application.Features.Surveys.Commands.SurveyUpdate;
using SylviaNG.Community.Application.Features.Surveys.Models;
using SylviaNG.Community.Application.Features.Surveys.Queries.SurveyAudienceGetAll;
using SylviaNG.Community.Application.Features.Surveys.Queries.SurveyGetAllPaged;
using SylviaNG.Community.Application.Features.Surveys.Queries.SurveyGetById;
using SylviaNG.Community.Application.Features.Surveys.Queries.SurveyQuestionGetAll;
using SylviaNG.Community.Application.Features.Surveys.Queries.SurveyResponseGetAllPaged;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Controllers
{
    [ApiController]
    [Route("community/survey")]
    public class SurveyController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SurveyController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<SurveyDetailResponse>>> GetPaged([FromQuery] PagedRequest request)
        {
            var result = await _mediator.Send(new SurveyGetAllPagedQuery(request));
            return Ok(result);
        }

        [HttpGet("{surveyId}")]
        public async Task<ActionResult<SurveyDetailResponse>> GetById(long surveyId)
        {
            var result = await _mediator.Send(new SurveyGetByIdQuery(surveyId));
            return Ok(result);
        }

        [Authorize(Policy = "HRAdminOnly")]
        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] SurveyCreateRequest request)
        {
            var id = await _mediator.Send(new SurveyCreateCommand(request));
            return Ok(id);
        }

        [Authorize(Policy = "HRAdminOnly")]
        [HttpPut("{surveyId}")]
        public async Task<ActionResult> Update(long surveyId, [FromBody] SurveyUpdateRequest request)
        {
            await _mediator.Send(new SurveyUpdateCommand(surveyId, request));
            return Ok();
        }

        [Authorize(Policy = "HRAdminOnly")]
        [HttpPut("{surveyId}/publish")]
        public async Task<ActionResult> Publish(long surveyId)
        {
            await _mediator.Send(new SurveyPublishCommand(surveyId));
            return Ok();
        }

        [Authorize(Policy = "HRAdminOnly")]
        [HttpPut("{surveyId}/close")]
        public async Task<ActionResult> Close(long surveyId)
        {
            await _mediator.Send(new SurveyCloseCommand(surveyId));
            return Ok();
        }

        [HttpGet("{surveyId}/questions")]
        public async Task<ActionResult<List<SurveyQuestionResponse>>> GetQuestions(long surveyId)
        {
            var result = await _mediator.Send(new SurveyQuestionGetAllQuery(surveyId));
            return Ok(result);
        }

        [Authorize(Policy = "HRAdminOnly")]
        [HttpPost("{surveyId}/questions")]
        public async Task<ActionResult<long>> AddQuestion(long surveyId, [FromBody] SurveyQuestionCreateRequest request)
        {
            var id = await _mediator.Send(new SurveyQuestionAddCommand(surveyId, request));
            return Ok(id);
        }

        [Authorize(Policy = "HRAdminOnly")]
        [HttpPut("{surveyId}/questions/{questionId}")]
        public async Task<ActionResult> UpdateQuestion(long surveyId, long questionId, [FromBody] SurveyQuestionUpdateRequest request)
        {
            await _mediator.Send(new SurveyQuestionUpdateCommand(surveyId, questionId, request));
            return Ok();
        }

        [Authorize(Policy = "HRAdminOnly")]
        [HttpDelete("{surveyId}/questions/{questionId}")]
        public async Task<ActionResult> DeleteQuestion(long surveyId, long questionId)
        {
            await _mediator.Send(new SurveyQuestionDeleteCommand(surveyId, questionId));
            return Ok();
        }

        [Authorize(Policy = "HRAdminOnly")]
        [HttpGet("{surveyId}/audience")]
        public async Task<ActionResult<List<SurveyAudienceResponse>>> GetAudience(long surveyId)
        {
            var result = await _mediator.Send(new SurveyAudienceGetAllQuery(surveyId));
            return Ok(result);
        }

        [Authorize(Policy = "HRAdminOnly")]
        [HttpPost("{surveyId}/audience")]
        public async Task<ActionResult<long>> AddAudience(long surveyId, [FromBody] SurveyAudienceCreateRequest request)
        {
            var id = await _mediator.Send(new SurveyAudienceAddCommand(surveyId, request));
            return Ok(id);
        }

        /// <summary>
        /// Submit a full survey response (all answers) in a single request. Open to any
        /// authenticated employee - not restricted to HRAdminOnly.
        /// </summary>
        [HttpPost("{surveyId}/responses")]
        public async Task<ActionResult<long>> SubmitResponse(long surveyId, [FromBody] SurveySubmissionRequest request)
        {
            var id = await _mediator.Send(new SurveyResponseSubmitCommand(surveyId, request));
            return Ok(id);
        }

        [Authorize(Policy = "HRAdminOnly")]
        [HttpGet("{surveyId}/responses")]
        public async Task<ActionResult<PagedResult<SurveySubmissionResponse>>> GetResponses(long surveyId, [FromQuery] PagedRequest request)
        {
            var result = await _mediator.Send(new SurveyResponseGetAllPagedQuery(surveyId, request));
            return Ok(result);
        }
    }
}
