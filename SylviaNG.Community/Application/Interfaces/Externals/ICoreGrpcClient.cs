using SylviaNG.Community.Application.Common.Models;

namespace SylviaNG.Community.Application.Interfaces.Externals
{
    public interface ICoreGrpcClient
    {
        Task<CoreBatchLookupResult> GetSitesAsync(List<long> siteIds);

        /// <summary>
        /// Batch-resolves master data (department/designation/site names) in a single RPC call.
        /// Any parameter may be null/empty to skip that lookup.
        /// </summary>
        Task<CoreBatchLookupResult> GetMasterDataAsync(
            List<long>? departmentIds = null,
            List<long>? designationIds = null,
            List<long>? siteIds = null);
    }
}
