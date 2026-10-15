using CourtTime.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CourtTime.Data;

/// <summary>
/// Creates the database schema and the two roles. In Development it also adds two demo
/// accounts and a demo facility so the app can be tried (and recorded) right after cloning.
/// </summary>
public static class DbInitializer
{
    public const string DemoManagerEmail = "manager@courttime.test";
    public const string DemoPlayerEmail = "player@courttime.test";
    public const string DemoPassword = "CourtTime#2026";

    public static async Task InitializeAsync(IServiceProvider services, bool seedDemoData)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        await db.Database.EnsureCreatedAsync();

        foreach (var role in Roles.All)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        if (!seedDemoData || await db.Facilities.AnyAsync())
        {
            return;
        }

        var manager = await CreateUserAsync(userManager, DemoManagerEmail, "Demo Manager", Roles.FacilityManager);
        await CreateUserAsync(userManager, DemoPlayerEmail, "Demo Player", Roles.Player);

        db.Facilities.Add(new Facility
        {
            Name = "CourtTime Demo Arena",
            Address = "Av. Example, 100 - Maringá, PR",
            ContactPhone = "+55 44 99999-0000",
            ManagerId = manager.Id,
            Courts =
            [
                new Court { Name = "Beach Court 1", Sport = SportType.BeachTennis, Surface = CourtSurface.Sand, HourlyPrice = 80m },
                new Court { Name = "Beach Court 2", Sport = SportType.BeachTennis, Surface = CourtSurface.Sand, HourlyPrice = 80m },
                new Court { Name = "Padel Court", Sport = SportType.Padel, Surface = CourtSurface.SyntheticGrass, HourlyPrice = 120m, OpensAt = new(8, 0) },
                new Court { Name = "Futsal Court", Sport = SportType.Futsal, Surface = CourtSurface.Indoor, HourlyPrice = 150m, OpensAt = new(8, 0), ClosesAt = new(22, 0) }
            ]
        });

        await db.SaveChangesAsync();
    }

    private static async Task<ApplicationUser> CreateUserAsync(
        UserManager<ApplicationUser> userManager, string email, string fullName, string role)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
        {
            user = new ApplicationUser { UserName = email, Email = email, EmailConfirmed = true, FullName = fullName };
            await userManager.CreateAsync(user, DemoPassword);
            await userManager.AddToRoleAsync(user, role);
        }

        return user;
    }
}
