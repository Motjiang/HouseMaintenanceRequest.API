using HouseMaintenanceRequest.API.Data;
using HouseMaintenanceRequest.API.Features.Property.Command;
using HouseMaintenanceRequest.API.Models.Domain;
using HouseMaintenanceRequest.API.Models.Enums;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HouseMaintenanceRequest.API.Features.Property.Handler
{
    public class CreatePropertyCommandHandler : IRequestHandler<CreatePropertyCommand, int>
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CreatePropertyCommandHandler(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<int> Handle(CreatePropertyCommand request, CancellationToken cancellationToken)
        {
            // Get logged-in user
            var currentUser = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);

            if (currentUser == null)
                throw new UnauthorizedAccessException("User not authenticated");

            // Validate landlord exists
            var landlord = await _context.Landlords
                .FirstOrDefaultAsync(x => x.LandlordId == request.landLordId, cancellationToken);

            if (landlord == null)
                throw new ArgumentException("Landlord not found");

            // Create property
            var property = new Models.Domain.Property
            {
                PropertyName = request.propertyName,
                Location = request.location,
                Status = EntityStatus.Pending,
                IsDeleted = false,
                LandlordId = landlord.LandlordId
            };

            _context.Properties.Add(property);
            await _context.SaveChangesAsync(cancellationToken);

            // Notify all admins
            var admins = await _userManager.GetUsersInRoleAsync("Admin");

            foreach (var admin in admins)
            {
                var notification = new Notification
                {
                    Title = "New Property Added",
                    Message = $"A new property '{property.PropertyName}' is awaiting approval.",
                    CreatedAt = DateTime.UtcNow,
                    RecipientUserId = admin.Id,
                    TargetController = "Property",
                    TargetAction = "Details"
                };

                _context.Notifications.Add(notification);
            }

            await _context.SaveChangesAsync(cancellationToken);

            return property.PropertyId;
        }
    }
}
