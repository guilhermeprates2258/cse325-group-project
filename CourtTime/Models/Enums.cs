namespace CourtTime.Models;

/// <summary>Sports a court can be booked for.</summary>
public enum SportType
{
    BeachTennis,
    Padel,
    Futsal,
    Volleyball,
    Tennis
}

/// <summary>Playing surface of a court.</summary>
public enum CourtSurface
{
    Sand,
    SyntheticGrass,
    Hardcourt,
    Clay,
    Indoor
}

/// <summary>Lifecycle of a booking.</summary>
public enum BookingStatus
{
    Confirmed,
    Cancelled,
    Completed,
    NoShow
}

/// <summary>Payment is tracked manually by the facility manager (no online payments in scope).</summary>
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
    Event,
    Other
}
