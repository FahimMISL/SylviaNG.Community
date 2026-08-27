using SylviaNG.Community.Application.Common.Models;
using SylviaNG.Community.Application.Interfaces.Externals;
using SylviaNG.Community.Application.Interfaces.Repositories;

namespace SylviaNG.Community.Infrastructure.Services
{
    // Local stand-in for the external Core microservice's gRPC BatchLookup. Used when
    // GrpcServices:CoreService:UseStub is enabled because the real Core service (which owns
    // department/designation/site master data) isn't reachable in this environment.
    //
    // Department/Designation names are resolved from this app's own local Department/Designation
    // tables (the same tables the Add Employee dropdown and Survey/Election audience targeting
    // already use) - this keeps what an admin picks in the UI and what gets displayed afterward
    // consistent, which a purely generated placeholder name could never guarantee. Falls back to
    // the old deterministic generator only for an id with no local row (e.g. ids seeded before
    // this table existed), so nothing regresses to a blank/placeholder-free result.
    public class StubCoreGrpcClient : ICoreGrpcClient
    {
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IDesignationRepository _designationRepository;

        private static readonly string[] DepartmentNames =
        {
            "Engineering", "Human Resources", "Sales & Marketing", "Finance",
            "Operations", "Customer Support", "Legal", "IT Infrastructure"
        };

        private static readonly string[] DesignationNames =
        {
            "Software Engineer", "Senior Software Engineer", "HR Manager", "Sales Executive",
            "Financial Analyst", "Operations Lead", "Support Specialist", "Legal Counsel",
            "System Administrator", "Team Lead"
        };

        private static readonly string[] SiteNames =
        {
            "Head Office - Dhaka", "Branch - Chattogram", "Branch - Sylhet", "Regional Office - Khulna"
        };

        public StubCoreGrpcClient(IDepartmentRepository departmentRepository, IDesignationRepository designationRepository)
        {
            _departmentRepository = departmentRepository;
            _designationRepository = designationRepository;
        }

        public Task<CoreBatchLookupResult> GetSitesAsync(List<long> siteIds)
        {
            return Task.FromResult(new CoreBatchLookupResult
            {
                Sites = ToLookup(siteIds, SiteNames, "SITE")
            });
        }

        public async Task<CoreBatchLookupResult> GetMasterDataAsync(
            List<long>? departmentIds = null,
            List<long>? designationIds = null,
            List<long>? siteIds = null)
        {
            return new CoreBatchLookupResult
            {
                Departments = await ToLookupFromLocalAsync(
                    departmentIds,
                    distinctIds => _departmentRepository.FindAsync(d => distinctIds.Contains(d.DepartmentId)),
                    d => d.DepartmentId, d => d.Name, DepartmentNames, "DEPT"),
                Designations = await ToLookupFromLocalAsync(
                    designationIds,
                    distinctIds => _designationRepository.FindAsync(d => distinctIds.Contains(d.DesignationId)),
                    d => d.DesignationId, d => d.Name, DesignationNames, "DESIG"),
                Sites = ToLookup(siteIds, SiteNames, "SITE")
            };
        }

        // find takes the deduplicated id list as a plain captured value (so the caller's lambda
        // can reference the entity's real id property directly, e.g. "d => distinctIds.Contains(
        // d.DepartmentId)") - EF Core can translate that to SQL, but it can NOT translate an
        // invocation of an arbitrary Func<TEntity,long> passed in as data (tried that first;
        // failed with "could not be translated", confirmed by live testing).
        private static async Task<List<EntityIdNameCodeResponse>> ToLookupFromLocalAsync<TEntity>(
            List<long>? ids,
            Func<List<long>, Task<IEnumerable<TEntity>>> find,
            Func<TEntity, long> getId,
            Func<TEntity, string> getName,
            string[] fallbackNames,
            string codePrefix)
        {
            if (ids is not { Count: > 0 })
                return new List<EntityIdNameCodeResponse>();

            var distinctIds = ids.Distinct().ToList();
            var localEntities = (await find(distinctIds)).ToDictionary(getId);

            return distinctIds.Select(id => new EntityIdNameCodeResponse
            {
                EntityId = id,
                Name = localEntities.TryGetValue(id, out var entity)
                    ? getName(entity)
                    : fallbackNames[(int)(Math.Abs(id - 1) % fallbackNames.Length)],
                Code = $"{codePrefix}{id}"
            }).ToList();
        }

        private static List<EntityIdNameCodeResponse> ToLookup(List<long>? ids, string[] names, string codePrefix)
        {
            if (ids is not { Count: > 0 })
                return new List<EntityIdNameCodeResponse>();

            return ids.Distinct().Select(id => new EntityIdNameCodeResponse
            {
                EntityId = id,
                Name = names[(int)(Math.Abs(id - 1) % names.Length)],
                Code = $"{codePrefix}{id}"
            }).ToList();
        }
    }
}
