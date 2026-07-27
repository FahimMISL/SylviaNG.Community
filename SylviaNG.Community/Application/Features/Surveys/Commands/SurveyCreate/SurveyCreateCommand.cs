using MediatR;
using SylviaNG.Community.Application.Features.Surveys.Models;

namespace SylviaNG.Community.Application.Features.Surveys.Commands.SurveyCreate
{
    public class SurveyCreateCommand : IRequest<long>
    {
        public SurveyCreateRequest Request { get; set; }

        public SurveyCreateCommand(SurveyCreateRequest request)
        {
            Request = request;
        }
    }
}
