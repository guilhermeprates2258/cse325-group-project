using System.ComponentModel.DataAnnotations;
using CourtTime.Data;

namespace CourtTime.Models;

/// <summary>A sports facility (arena or club) owned by a Facility Manager.</summary>
public class Facility
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required, StringLength(200)]
    public string Address { get; set; } = string.Empty;

    [Phone, StringLength(20)]
    public string? ContactPhone { get; set; }

    /// <summary>Identity user id of the manager who owns this facility.</summary>
    [Required]
    public string ManagerId { get; set; } = string.Empty;
    public ApplicationUser? Manager { get; set; }

    public ICollection<Court> Courts { get; set; } = new List<Court>();
}
