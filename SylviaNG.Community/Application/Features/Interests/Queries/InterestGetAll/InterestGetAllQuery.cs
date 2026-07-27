using MediatR;
using SylviaNG.Community.Application.Features.Interests.Models;

namespace SylviaNG.Community.Application.Features.Interests.Queries.InterestGetAll
{
    public class InterestGetAllQuery : IRequest<List<InterestResponse>>
    {
    }
}
