using SylviaNG.Community.Application.Common.Models;
using SylviaNG.Community.Application.Features.Employees.Models;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.Domain.Enums;

namespace SylviaNG.Community.Application.Mappings
{
    /// <summary>
    /// Manual mapping methods for the Employee entity, following the same pattern as
    /// AnnouncementMapper. Master-data names (department/designation/site/grade) are
    /// resolved by the service via ICoreGrpcClient and passed in as a CoreBatchLookupResult
    /// so these methods stay synchronous and pure.
    /// </summary>
    public static class EmployeeMapper
    {
        public static Employee ToEntity(this EmployeeCreateRequest request) => new()
        {
            EmployeeName = request.EmployeeName,
            Email = request.Email,
            DesignatioId = request.DesignationId,
            DepartmentId = request.DepartmentId,
            SiteId = request.SiteId,
            IsActive = true
        };

        public static void ApplyProfileUpdate(this Employee entity, EmployeeUpdateProfileRequest request)
        {
            entity.Bio = request.Bio;
            entity.Skills = request.Skills;
            entity.Interests = request.Interests;
            entity.Achievements = request.Achievements;
            entity.CommunityContributions = request.CommunityContributions;
            entity.Phone = request.Phone;
            entity.Email = request.Email;
            entity.Extension = request.Extension;
            entity.PhoneVisibility = request.PhoneVisibility;
            entity.EmailVisibility = request.EmailVisibility;
            entity.ExtensionVisibility = request.ExtensionVisibility;
        }

        public static EmployeeResponse ToResponse(
            this Employee entity,
            long? viewerEmployeeId,
            bool viewerIsHrAdmin,
            CoreBatchLookupResult lookups,
            List<EmployeeContactLink> contactLinks)
        {
            var isOwnProfile = viewerEmployeeId.HasValue && viewerEmployeeId.Value == entity.EmployeeId;
            var canSeePrivate = isOwnProfile || viewerIsHrAdmin;

            return new EmployeeResponse
            {
                EmployeeId = entity.EmployeeId,
                EmployeeCode = entity.EmployeeCode,
                EmployeeName = entity.EmployeeName,
                DesignationId = entity.DesignatioId,
                DesignationName = FindName(lookups.Designations, entity.DesignatioId),
                DepartmentId = entity.DepartmentId,
                DepartmentName = FindName(lookups.Departments, entity.DepartmentId),
                SiteId = entity.SiteId,
                SiteName = FindName(lookups.Sites, entity.SiteId),
                Division = entity.Division,
                Bio = entity.Bio,
                Skills = SplitList(entity.Skills),
                Interests = SplitList(entity.Interests),
                Achievements = SplitList(entity.Achievements),
                CommunityContributions = SplitList(entity.CommunityContributions),
                Badges = new List<string>(),
                RecentRecognitions = new List<string>(),
                PhotoUrl = entity.PhotoUrl,
                CoverPhotoUrl = entity.CoverPhotoUrl,
                IsActive = entity.IsActive,
                Phone = canSeePrivate || entity.PhoneVisibility == ContactVisibilityEnum.Public ? entity.Phone : null,
                Email = canSeePrivate || entity.EmailVisibility == ContactVisibilityEnum.Public ? entity.Email : null,
                Extension = canSeePrivate || entity.ExtensionVisibility == ContactVisibilityEnum.Public ? entity.Extension : null,
                PhoneVisibility = entity.PhoneVisibility,
                EmailVisibility = entity.EmailVisibility,
                ExtensionVisibility = entity.ExtensionVisibility,
                ContactLinks = contactLinks
                    .Where(cl => canSeePrivate || cl.Visibility == ContactVisibilityEnum.Public)
                    .Select(cl => new EmployeeContactLinkResponse
                    {
                        Id = cl.EmployeeContactLinkId,
                        Platform = cl.Platform,
                        Url = cl.Url,
                        Visibility = cl.Visibility
                    }).ToList(),
                IsOwnProfile = isOwnProfile
            };
        }

        public static EmployeeDirectoryCardResponse ToDirectoryCard(this Employee entity, CoreBatchLookupResult lookups) => new()
        {
            EmployeeId = entity.EmployeeId,
            EmployeeCode = entity.EmployeeCode,
            EmployeeName = entity.EmployeeName,
            PhotoUrl = entity.PhotoUrl,
            DesignationId = entity.DesignatioId,
            DesignationName = FindName(lookups.Designations, entity.DesignatioId),
            DepartmentId = entity.DepartmentId,
            DepartmentName = FindName(lookups.Departments, entity.DepartmentId),
            SiteId = entity.SiteId,
            SiteName = FindName(lookups.Sites, entity.SiteId),
            Skills = SplitList(entity.Skills)
        };

        public static EmployeeManagementRowResponse ToManagementRow(this Employee entity, CoreBatchLookupResult lookups, HashSet<long> employeeIdsWithCredentials) => new()
        {
            EmployeeId = entity.EmployeeId,
            EmployeeCode = entity.EmployeeCode,
            EmployeeName = entity.EmployeeName,
            Email = entity.Email,
            DesignationId = entity.DesignatioId,
            DesignationName = FindName(lookups.Designations, entity.DesignatioId),
            DepartmentId = entity.DepartmentId,
            DepartmentName = FindName(lookups.Departments, entity.DepartmentId),
            SiteId = entity.SiteId,
            SiteName = FindName(lookups.Sites, entity.SiteId),
            IsActive = entity.IsActive,
            HasCredential = employeeIdsWithCredentials.Contains(entity.EmployeeId)
        };

        private static string? FindName(List<EntityIdNameCodeResponse> items, long? id) =>
            id.HasValue ? items.FirstOrDefault(i => i.EntityId == id.Value)?.Name : null;

        private static List<string> SplitList(string? csv) =>
            string.IsNullOrWhiteSpace(csv)
                ? new List<string>()
                : csv.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();
    }
}
