using Grpc.Core;
using SylviaNG.Community.Application.Common.Models;
using SylviaNG.Community.Application.Interfaces.Externals;
using SylviaNG.Community.Grpc.Generated.Core;

namespace SylviaNG.Community.Infrastructure.Services
{
    public class CoreGrpcClient : ICoreGrpcClient
    {
        private readonly CoreService.CoreServiceClient _client;
        private readonly ILogger<CoreGrpcClient> _logger;

        public CoreGrpcClient(CoreService.CoreServiceClient client, ILogger<CoreGrpcClient> logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task<CoreBatchLookupResult> GetSitesAsync(List<long> siteIds)
        {
            try
            {
                var request = new BatchLookupRequest();
                request.SiteIds.AddRange(siteIds);

                var response = await _client.BatchLookupAsync(request);

                var result = new CoreBatchLookupResult
                {
                    Sites = response.Sites.Select(s => new EntityIdNameCodeResponse
                    {
                        EntityId = s.Id,
                        Name = s.Name,
                        Code = s.Code
                    }).ToList()
                };

                return result;
            }
            catch (RpcException ex)
            {
                _logger.LogError(ex, "gRPC error calling CoreService.BatchLookup");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error calling CoreService.BatchLookup");
                throw;
            }
        }

        public async Task<CoreBatchLookupResult> GetMasterDataAsync(
            List<long>? departmentIds = null,
            List<long>? designationIds = null,
            List<long>? siteIds = null)
        {
            try
            {
                var request = new BatchLookupRequest();
                if (departmentIds is { Count: > 0 }) request.DepartmentIds.AddRange(departmentIds);
                if (designationIds is { Count: > 0 }) request.DesignationIds.AddRange(designationIds);
                if (siteIds is { Count: > 0 }) request.SiteIds.AddRange(siteIds);

                var response = await _client.BatchLookupAsync(request);

                return new CoreBatchLookupResult
                {
                    Departments = response.Departments.Select(d => new EntityIdNameCodeResponse
                    {
                        EntityId = d.Id,
                        Name = d.Name,
                        Code = d.Code
                    }).ToList(),
                    Designations = response.Designations.Select(d => new EntityIdNameCodeResponse
                    {
                        EntityId = d.Id,
                        Name = d.Name,
                        Code = d.Code
                    }).ToList(),
                    Sites = response.Sites.Select(s => new EntityIdNameCodeResponse
                    {
                        EntityId = s.Id,
                        Name = s.Name,
                        Code = s.Code
                    }).ToList()
                };
            }
            catch (RpcException ex)
            {
                _logger.LogError(ex, "gRPC error calling CoreService.BatchLookup");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error calling CoreService.BatchLookup");
                throw;
            }
        }
    }
}
