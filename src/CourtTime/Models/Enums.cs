namespace CourtTime.Models;

/// <summary>Sports supported by a court.</summary>
public enum SportType
{
    BeachTennis,
    Padel,
    Futsal
}

/// <summary>Lifecycle of a booking.</summary>
public enum BookingStatus
{
    Confirmed,
    Cancelled,
    Completed,
    NoShow
}

/// <summary>Payment is tracked manually by the manager (no online payments in scope).</summary>
public enum PaymentStatus
{
    Pending,
    Paid
}

/// <summary>Why a manager blocked a time slot.</summary>
public enum BlockReason
{
    Class,
    Maintenance,
    Event
}
