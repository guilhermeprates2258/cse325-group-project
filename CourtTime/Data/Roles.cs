namespace CourtTime.Data;

/// <summary>Role names used with [Authorize(Roles = ...)].</summary>
public static class Roles
{
    public const string Player = "Player";
    public const string FacilityManager = "FacilityManager";

    public static readonly string[] All = [Player, FacilityManager];
}
