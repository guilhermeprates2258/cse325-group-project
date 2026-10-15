using System.ComponentModel.DataAnnotations;
using CourtTime.Models;
using Microsoft.AspNetCore.Identity;

namespace CourtTime.Data;

/// <summary>Identity user extended with the profile fields CourtTime needs.</summary>
public class ApplicationUser : IdentityUser
{
    [StringLength(100)]
    public string? FullName { get; set; }

    public SportType? PreferredSport { get; set; }

    public List<Booking> Bookings { get; set; } = [];
}
