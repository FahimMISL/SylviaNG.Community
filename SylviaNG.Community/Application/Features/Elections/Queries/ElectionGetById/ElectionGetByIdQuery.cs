using MediatR;
using SylviaNG.Community.Application.Features.Elections.Models;

namespace SylviaNG.Community.Application.Features.Elections.Queries.ElectionGetById
{
    public class ElectionGetByIdQuery : IRequest<ElectionResponse>
    {
        public long ElectionId { get; set; }

        public ElectionGetByIdQuery(long electionId)
        {
            ElectionId = electionId;
        }
    }
}
