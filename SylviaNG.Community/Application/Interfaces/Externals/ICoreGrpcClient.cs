using SylviaNG.Community.Application.Common.Models;

namespace SylviaNG.Community.Application.Interfaces.Externals
{
    public interface ICoreGrpcClient
    {
        Task<CoreBatchLookupResult> GetSitesAsync(List<long> siteIds);
    }
}
