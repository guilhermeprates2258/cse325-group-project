using System.ComponentModel.DataAnnotations;

namespace CourtTime.Models;

/// <summary>A period in which a court cannot be booked (class, maintenance, event).</summary>
public class BlockedSlot
{
    public int Id { get; set; }

    public int CourtId { get; set; }
    public Court? Court { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public BlockReason Reason { get; set; }

    [StringLength(200)]
    public string? Notes { get; set; }

    public bool OverlapsWith(DateTime start, DateTime end) =>
        StartTime < end && start < EndTime;
}
