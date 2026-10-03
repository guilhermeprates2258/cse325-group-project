using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using CourtTime.Models;

namespace CourtTime.Data;

/// <summary>
/// A CourtTime user (Player or Facility Manager). Extends the ASP.NET Core
/// Identity user with the profile fields described in the project proposal.
/// Phone number is inherited from <see cref="IdentityUser.PhoneNumber"/>.
/// </summary>
public class ApplicationUser : IdentityUser
{
    [Required, StringLength(100)]
    public string FullName { get; set; } = string.Empty;

    public SportType? PreferredSport { get; set; }

    /// <summary>Facilities this user manages (Facility Managers only).</summary>
    public ICollection<Facility> ManagedFacilities { get; set; } = new List<Facility>();

    /// <summary>Bookings made by this user as a player.</summary>
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
