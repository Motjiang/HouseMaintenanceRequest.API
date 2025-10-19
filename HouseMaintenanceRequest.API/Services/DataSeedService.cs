using HouseMaintenanceRequest.API.Data;
using HouseMaintenanceRequest.API.Models.Domain;
using HouseMaintenanceRequest.API.Models.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HouseMaintenanceRequest.API.Services
{
    public class DataSeedService
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;

        public DataSeedService(
            RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context,
            IConfiguration config)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _context = context;
            _config = config;
        }

        public async Task SeedAsync()
        {
            await SeedRolesAsync();
            await SeedUsersAsync();
        }

        private async Task SeedRolesAsync()
        {
            string[] roles =
            {
                Constants.Role_Admin,
                Constants.Role_Landlord,
                Constants.Role_Tenant,
                Constants.Role_MaintenanceCompany
            };

            foreach (var role in roles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                    await _roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        private async Task SeedUsersAsync()
        {
            // Admin
            await CreateUserWithDomainAsync(
                _config["SeedData:Admin:Email"],
                _config["SeedData:Admin:Password"],
                Constants.Role_Admin,
                "System", "Admin");

            // Landlord
            await CreateUserWithDomainAsync(
                _config["SeedData:Landlord:Email"],
                _config["SeedData:Landlord:Password"],
                Constants.Role_Landlord,
                "Default", "Landlord");

            // Tenant
            await CreateUserWithDomainAsync(
                _config["SeedData:Tenant:Email"],
                _config["SeedData:Tenant:Password"],
                Constants.Role_Tenant,
                "Default", "Tenant");

            // MaintenanceCompany
            await CreateUserWithDomainAsync(
                _config["SeedData:MaintenanceCompany:Email"],
                _config["SeedData:MaintenanceCompany:Password"],
                Constants.Role_MaintenanceCompany,
                "Default", "MaintenanceCo");
        }

        private async Task CreateUserWithDomainAsync(
            string? email, string? password, string role, string firstName, string lastName)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return;

            var existingUser = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (existingUser != null)
                return;

            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                DateCreated = DateTime.UtcNow,
                IsActive = true,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded)
                throw new Exception($"Failed to create {role}: {string.Join(", ", result.Errors.Select(e => e.Description))}");

            await _userManager.AddToRoleAsync(user, role);

            // Add domain-specific entry
            switch (role)
            {
                case Constants.Role_Landlord:
                    if (!await _context.Landlords.AnyAsync(l => l.ApplicationUserId == user.Id))
                    {
                        _context.Landlords.Add(new Landlord
                        {
                            ApplicationUserId = user.Id,
                            PhysicalAddress = "Default Address",
                            Status = EntityStatus.Active,
                            IsDeleted = false
                        });
                    }
                    break;

                case Constants.Role_Tenant:
                    if (!await _context.Tenants.AnyAsync(t => t.ApplicationUserId == user.Id))
                    {
                        _context.Tenants.Add(new Tenant
                        {
                            ApplicationUserId = user.Id,
                            Status = EntityStatus.Active,
                            IsDeleted = false
                        });
                    }
                    break;

                case Constants.Role_MaintenanceCompany:
                    if (!await _context.MaintenanceCompanies.AnyAsync(m => m.ApplicationUserId == user.Id))
                    {
                        _context.MaintenanceCompanies.Add(new MaintenanceCompany
                        {
                            ApplicationUserId = user.Id,
                            CompanyName = "Default Maintenance Co",
                            TypeOfMaintenance = MaintenanceType.General,
                            Status = EntityStatus.Active,
                            IsDeleted = false
                        });
                    }
                    break;
            }

            await _context.SaveChangesAsync();
        }
    }
}
