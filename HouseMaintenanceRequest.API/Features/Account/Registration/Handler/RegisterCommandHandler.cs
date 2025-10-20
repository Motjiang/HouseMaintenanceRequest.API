using HouseMaintenanceRequest.API.Data;
using HouseMaintenanceRequest.API.Features.Account.Registration.Command;
using HouseMaintenanceRequest.API.Models.Domain;
using HouseMaintenanceRequest.API.Models.DTOs.Account;
using HouseMaintenanceRequest.API.Models.Enums;
using HouseMaintenanceRequest.API.Services;
using HouseMaintenanceRequest.API.System_Communication.Hubs;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace HouseMaintenanceRequest.API.Features.Account.Registration.Handler
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, bool>
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IHubContext<NotificationHub> _hubContext;

        public RegisterCommandHandler(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IHubContext<NotificationHub> hubContext)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _hubContext = hubContext;
        }

        public async Task<bool> Handle(RegisterCommand command, CancellationToken cancellationToken)
        {
            var dto = command.RegisterDto;
            try
            {
                // Ensure role exists
                if (!await _roleManager.RoleExistsAsync(dto.Role))
                    await _roleManager.CreateAsync(new IdentityRole(dto.Role));

                // Role-specific existence checks
                switch (dto.Role)
                {
                    case Constants.Role_Landlord:
                    case Constants.Role_Tenant:
                        if (await _userManager.Users.AnyAsync(u => u.UserName == dto.Email || u.Email == dto.Email))
                            return false;
                        break;

                    case Constants.Role_MaintenanceCompany:
                        if (await _context.MaintenanceCompanies.AnyAsync(c => c.CompanyName == dto.CompanyName))
                            return false;
                        break;
                }

                // Create base ApplicationUser
                var user = new ApplicationUser
                {
                    UserName = dto.Email,
                    Email = dto.Email,
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    DateCreated = DateTime.Now,
                    IsActive = true,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user, dto.Password);
                if (!result.Succeeded)
                    throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

                await _userManager.AddToRoleAsync(user, dto.Role);

                // Role-specific creation + notifications
                switch (dto.Role)
                {
                    case Constants.Role_Landlord:
                        var landlord = new Landlord
                        {
                            ApplicationUserId = user.Id,
                            PhysicalAddress = dto.PhysicalAddress ?? "Default Address",
                            BusinessDocumentPath = dto.BusinessDocumentPath,
                            Status = EntityStatus.Pending,
                            IsDeleted = false
                        };
                        _context.Landlords.Add(landlord);

                        // Notify user
                        await _hubContext.Clients
                            .All.SendAsync("SendNotificationToUser", user.Id,
                                "Welcome!",
                                "Your landlord account has been created and is awaiting document approval.");

                        // Notify admins
                        await _hubContext.Clients
                            .All.SendAsync("NotifyAdmins",
                                "New Account Pending Approval",
                                $"New landlord registered: {user.FirstName} {user.LastName}. Please review their documents for approval.");
                        break;

                    case Constants.Role_MaintenanceCompany:
                        var company = new MaintenanceCompany
                        {
                            ApplicationUserId = user.Id,
                            CompanyName = dto.CompanyName ?? "Default Company",
                            TypeOfMaintenance = dto.TypeOfMaintenance ?? MaintenanceType.General,
                            DocumentPath = dto.DocumentPath,
                            Status = EntityStatus.Pending,
                            IsDeleted = false
                        };
                        _context.MaintenanceCompanies.Add(company);

                        await _hubContext.Clients
                            .All.SendAsync("SendNotificationToUser", user.Id,
                                "Welcome!",
                                "Your maintenance company account has been created and is awaiting approval.");

                        await _hubContext.Clients
                            .All.SendAsync("NotifyAdmins",
                                "New Account Pending Approval",
                                $"New maintenance company registered: {company.CompanyName}. Please review their documents for approval.");
                        break;

                    case Constants.Role_Tenant:
                        var tenant = new Tenant
                        {
                            ApplicationUserId = user.Id,
                            Status = EntityStatus.Active,
                            IsDeleted = false
                        };
                        _context.Tenants.Add(tenant);

                        await _hubContext.Clients
                            .All.SendAsync("SendNotificationToUser", user.Id,
                                "Welcome!",
                                "Your tenant account has been successfully created.");
                        break;
                }

                await _context.SaveChangesAsync(cancellationToken);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Registration failed: {ex.Message}");
                return false;
            }
        }
    }
}
