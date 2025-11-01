using HouseMaintenanceRequest.API.Data;
using HouseMaintenanceRequest.API.Features.Property.Command;
using HouseMaintenanceRequest.API.Models.Domain;
using HouseMaintenanceRequest.API.Models.DTOs.Account;
using HouseMaintenanceRequest.API.Services;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HouseMaintenanceRequest.API.Features.Property.Handler
{
    public class CreatePropertyCommandHandler : IRequestHandler<CreatePropertyCommand, Models.Domain.Property>
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly EmailService _emailService;

        public CreatePropertyCommandHandler(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IHttpContextAccessor httpContextAccessor,
            EmailService emailService)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _emailService = emailService;
        }

        public async Task<Models.Domain.Property> Handle(CreatePropertyCommand request, CancellationToken cancellationToken)
        {
            // ✅ Get current logged-in landlord
            var currentLandlordUser = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);

            if (currentLandlordUser == null)
                throw new UnauthorizedAccessException("User not authenticated");

            var landlord = await _context.Landlords
                .Include(l => l.ApplicationUser)
                .FirstOrDefaultAsync(l => l.ApplicationUserId == currentLandlordUser.Id, cancellationToken);

            if (landlord == null)
                throw new InvalidOperationException("Landlord not found");

            // ✅ Create new property
            var property = new Models.Domain.Property
            {
                PropertyName = request.propertyName,
                Location = request.location,
                Status = Models.Enums.EntityStatus.Pending, // Default pending
                IsDeleted = false,
                LandlordId = landlord.LandlordId
            };

            _context.Properties.Add(property);
            await _context.SaveChangesAsync(cancellationToken);

            // Send email (fire-and-forget)
            if (!string.IsNullOrWhiteSpace(currentLandlordUser.Email))
            {
                var emailDto = new SendEmailDto(
                    currentLandlordUser.Email,
                    "Property Pending Approval",
                    $"<p>Dear {currentLandlordUser.FirstName} {currentLandlordUser.LastName},</p>" +
                    $"<p>Your property <strong>{property.PropertyName}</strong> has been successfully created and is currently <strong>pending approval</strong>.</p>"
                );

                _ = Task.Run(async () => await _emailService.SendEmailAsync(emailDto));
            }

            return property;
        }
    }
}
