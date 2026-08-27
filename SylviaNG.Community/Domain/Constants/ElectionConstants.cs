namespace SylviaNG.Community.Domain.Constants;

/// <summary>
/// Well-known values for Election's plain-string fields (see the "kept as plain strings"
/// convention documented on the Election entity). Centralized here so ElectionService,
/// ElectionEligibilityService and the background auto-close service all agree on the exact
/// values without drifting.
/// </summary>
public static class ElectionAudienceScope
{
    public const string Organization = "Organization";
    public const string Branch = "Branch";
    public const string Department = "Department";
    public const string Team = "Team";
    public const string SelectedEmployees = "SelectedEmployees";
}

public static class ElectionStatus
{
    public const string Draft = "Draft";
    public const string Open = "Open";
    public const string Active = "Active";
    public const string Closed = "Closed";

    public static readonly string[] Votable = { Open, Active };
}
