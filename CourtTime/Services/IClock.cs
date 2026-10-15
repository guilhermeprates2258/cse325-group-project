using Microsoft.Extensions.Configuration;

namespace CourtTime.Services;

/// <summary>
/// Gives the current date and time in the facility's time zone.
/// Booking times are stored as local facility times, so "now" must use the same zone
/// even when the server (for example Azure App Service) runs in UTC.
/// </summary>
public interface IClock
{
    DateTime Now { get; }
    DateOnly Today => DateOnly.FromDateTime(Now);
}

public sealed class FacilityClock : IClock
{
    private readonly TimeZoneInfo timeZone;

    public FacilityClock(IConfiguration configuration)
    {
        var id = configuration["CourtTime:TimeZone"] ?? "America/Sao_Paulo";
        timeZone = TimeZoneInfo.TryFindSystemTimeZoneById(id, out var zone) ? zone : TimeZoneInfo.Local;
    }

    public DateTime Now => DateTime.SpecifyKind(
        TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone),
        DateTimeKind.Unspecified);
}
