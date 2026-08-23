namespace SylviaNG.Community.Application.Features.Surveys
{
    /// <summary>
    /// Whitelisted values for SurveyAudience.AudienceType. The entity/DTOs store this as a
    /// plain string (no C# enum anywhere in the Surveys feature, matching SurveyType/QuestionType/
    /// Status conventions elsewhere in this feature) - this class is the single source of truth
    /// for the values SurveyAudienceAddValidator accepts.
    /// </summary>
    public static class SurveyAudienceTypes
    {
        public const string EntireCompany = "EntireCompany";
        public const string Department = "Department";
        public const string Branch = "Branch";

        public static readonly string[] All = { EntireCompany, Department, Branch };
    }
}
