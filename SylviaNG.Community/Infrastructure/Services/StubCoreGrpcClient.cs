using SylviaNG.Community.Application.Common.Models;
using SylviaNG.Community.Application.Interfaces.Externals;

namespace SylviaNG.Community.Infrastructure.Services
{
    // Local stand-in for the external Core microservice's gRPC BatchLookup. Used when
    // GrpcServices:CoreService:UseStub is enabled because the real Core service (which owns
    // department/designation/site/grade master data) isn't reachable in this environment.
    // Generates deterministic, human-readable names by id so the UI shows plausible values
    // instead of falling back to "Department #1" style placeholders.
    public class StubCoreGrpcClient : ICoreGrpcClient
    {
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

        private static readonly string[] GradeNames =
        {
            "Grade A", "Grade B", "Grade C", "Grade D"
        };

        public Task<CoreBatchLookupResult> GetSitesAsync(List<long> siteIds)
        {
            return Task.FromResult(new CoreBatchLookupResult
            {
                Sites = ToLookup(siteIds, SiteNames, "SITE")
            });
        }

        public Task<CoreBatchLookupResult> GetMasterDataAsync(
            List<long>? departmentIds = null,
            List<long>? designationIds = null,
            List<long>? gradeIds = null,
            List<long>? siteIds = null)
        {
            return Task.FromResult(new CoreBatchLookupResult
            {
                Departments = ToLookup(departmentIds, DepartmentNames, "DEPT"),
                Designations = ToLookup(designationIds, DesignationNames, "DESIG"),
                Grades = ToLookup(gradeIds, GradeNames, "GRADE"),
                Sites = ToLookup(siteIds, SiteNames, "SITE")
            });
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
