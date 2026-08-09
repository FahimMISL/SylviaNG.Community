using MediatR;
using SylviaNG.Community.Application.Features.Recognitions.Models;

namespace SylviaNG.Community.Application.Features.Recognitions.Queries.RecognitionGetById
{
    public class RecognitionGetByIdQuery : IRequest<RecognitionResponse>
    {
        public long RecognitionId { get; set; }

        public RecognitionGetByIdQuery(long recognitionId)
        {
            RecognitionId = recognitionId;
        }
    }
}
