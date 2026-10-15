using System.Globalization;
using CourtTime.Models;

namespace CourtTime.Components.Shared;

/// <summary>Display helpers shared by several pages.</summary>
public static class Formatting
{
    private static readonly CultureInfo Culture = CultureInfo.GetCultureInfo("en-US");

    public static string Sport(SportType sport) => sport switch
    {
        SportType.BeachTennis => "Beach tennis",
        _ => sport.ToString()
    };

    public static string Surface(CourtSurface surface) => surface switch
    {
        CourtSurface.SyntheticGrass => "Synthetic grass",
        _ => surface.ToString()
    };

    public static string Price(decimal value) => value.ToString("C", Culture);

    public static string Day(DateTime value) => value.ToString("ddd, MMM d", Culture);

    public static string Day(DateOnly value) => value.ToString("dddd, MMMM d", Culture);

    public static string Hour(DateTime value) => value.ToString("HH:mm", Culture);

    public static string Status(BookingStatus status) => status switch
    {
        BookingStatus.NoShow => "No-show",
        _ => status.ToString()
    };
}
