using MediatR;
using SylviaNG.Community.Application.Features.Recognitions.Models;

namespace SylviaNG.Community.Application.Features.Recognitions.Commands.RecognitionCreate
{
    public class RecognitionCreateCommand : IRequest<long>
    {
        public RecognitionCreateRequest Request { get; set; }

        public RecognitionCreateCommand(RecognitionCreateRequest request)
        {
            Request = request;
        }
    }
}
