using HouseMaintenanceRequest.API.Data;
using HouseMaintenanceRequest.API.Features.Property_View.Command;
using HouseMaintenanceRequest.API.Models.Domain;
using HouseMaintenanceRequest.API.Models.DTOs.Account;
using HouseMaintenanceRequest.API.Models.DTOs.Property_View;
using HouseMaintenanceRequest.API.Models.Enums;
using HouseMaintenanceRequest.API.Services;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HouseMaintenanceRequest.API.Features.Property_View.Handler
{
    public class CreatePropertyViewRequestCommandHandler
            : IRequestHandler<CreatePropertyViewRequestCommand, int>
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly EmailService _emailService;

        public CreatePropertyViewRequestCommandHandler(
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

        public async Task<int> Handle(CreatePropertyViewRequestCommand request, CancellationToken cancellationToken)
        {
            // Get logged-in user
            var currentUser = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);

            if (currentUser == null)
                throw new UnauthorizedAccessException("User not authenticated");

            // Validate tenant
            var tenant = await _context.Tenants
                .Include(x => x.ApplicationUser)
                .FirstOrDefaultAsync(x => x.TenantId == request.tenantId, cancellationToken);

            if (tenant == null)
                throw new ArgumentException("Tenant not found");

            // Validate property
            var property = await _context.Properties
                .Include(x => x.Landlord)
                .ThenInclude(x => x.ApplicationUser)
                .FirstOrDefaultAsync(x => x.PropertyId == request.propertyId, cancellationToken);

            if (property == null)
                throw new ArgumentException("Property not found");

            // Create property view request
            var viewRequest = new Models.Domain.PropertyViewRequest
            {
                TenantId = request.tenantId,
                PropertyId = request.propertyId,
                RequestedAt = DateTime.UtcNow,
                ScheduledAt = request.scheduledAt,
                Status = ViewRequestStatus.Pending
            };

            _context.PropertyViewRequests.Add(viewRequest);
            await _context.SaveChangesAsync(cancellationToken);

            // 1️⃣ Notify property landlord
            var landlordNotification = new Notification
            {
                Title = "Property View Scheduled",
                Message = $"{tenant.ApplicationUser.FirstName} scheduled a viewing for '{property.PropertyName}'",
                CreatedAt = DateTime.UtcNow,
                RecipientUserId = property.Landlord.ApplicationUserId,
                TargetController = "Property",
                TargetAction = "Details"
            };

            _context.Notifications.Add(landlordNotification);

            // 2️⃣ Notify tenant (optional app notification)
            var tenantNotification = new Notification
            {
                Title = "Viewing Requested",
                Message = $"Your viewing request for '{property.PropertyName}' is pending.",
                CreatedAt = DateTime.UtcNow,
                RecipientUserId = tenant.ApplicationUserId,
                TargetController = "Property",
                TargetAction = "Details"
            };

            _context.Notifications.Add(tenantNotification);

            await _context.SaveChangesAsync(cancellationToken);

            // 3️⃣ Send email to landlord
            _ = Task.Run(async () =>
            {
                await _emailService.SendEmailAsync(new SendEmailDto(
                    property.Landlord.ApplicationUser.Email,
                    "Property Viewing Request",
                    $"{tenant.ApplicationUser.FirstName} has scheduled a viewing for {property.PropertyName} on {request.scheduledAt}"
                ));
            });

            // 4️⃣ Send email to tenant
            _ = Task.Run(async () =>
            {
                await _emailService.SendEmailAsync(new SendEmailDto(
                    tenant.ApplicationUser.Email,
                    "Viewing Scheduled",
                    $"You scheduled a viewing for {property.PropertyName} on {request.scheduledAt}"
                ));
            });

            return viewRequest.ViewRequestId;
        }
    }
}
