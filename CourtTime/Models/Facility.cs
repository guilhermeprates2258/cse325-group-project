using System.ComponentModel.DataAnnotations;
using CourtTime.Data;

namespace CourtTime.Models;

/// <summary>A sports facility (arena or club) that owns one or more courts.</summary>
public class Facility
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(200)]
    public string? Address { get; set; }

    [RegularExpression(@"^[0-9+()\-\s]{7,30}$", ErrorMessage = "Enter a valid phone number.")]
    public string? ContactPhone { get; set; }

    /// <summary>The Facility Manager who owns this facility. Only this user can edit its courts.</summary>
    public string? ManagerId { get; set; }
    public ApplicationUser? Manager { get; set; }

    public List<Court> Courts { get; set; } = [];
}
