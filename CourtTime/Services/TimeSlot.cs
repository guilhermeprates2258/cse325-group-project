namespace CourtTime.Services;

public enum SlotState
{
    Free,
    Booked,
    Blocked,
    Past
}

/// <summary>One bookable hour on a court's daily calendar.</summary>
public sealed record TimeSlot(DateTime Start, DateTime End, SlotState State, bool IsMine = false);

/// <summary>Outcome of a booking operation, with a message that is safe to show to the user.</summary>
public sealed record BookingResult(bool Succeeded, string Message)
{
    public static BookingResult Ok(string message) => new(true, message);
    public static BookingResult Fail(string message) => new(false, message);
}
