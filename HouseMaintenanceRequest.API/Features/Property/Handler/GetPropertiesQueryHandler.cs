using HouseMaintenanceRequest.API.Data;
using HouseMaintenanceRequest.API.Features.Property.Query;
using HouseMaintenanceRequest.API.Models.Domain;
using HouseMaintenanceRequest.API.Models.DTOs;
using HouseMaintenanceRequest.API.Models.DTOs.Property;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HouseMaintenanceRequest.API.Features.Property.Handler
{
    public class GetPropertiesQueryHandler : IRequestHandler<GetPropertiesQuery, IEnumerable<PropertyDto>>
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetPropertiesQueryHandler(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<PropertyDto>> Handle(GetPropertiesQuery request, CancellationToken cancellationToken)
        {
            // ✅ Get logged in user
            var currentUser = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);

            if (currentUser == null)
                throw new UnauthorizedAccessException("User not authenticated");

            // ✅ Get role(s)
            var roles = await _userManager.GetRolesAsync(currentUser);

            // ✅ Initial query
            IQueryable<Models.Domain.Property> query = _context.Properties
                .Include(p => p.Landlord)
                    .ThenInclude(l => l.ApplicationUser)
                .Include(p => p.Tenant)
                    .ThenInclude(t => t.ApplicationUser);

            List<Models.Domain.Property> properties;

            // ========== ADMIN ==========
            if (roles.Contains("Admin"))
            {
                properties = await query.ToListAsync(cancellationToken);
                return properties.Select(p => MapToDto(p));
            }

            // ========== LANDLORD ==========
            if (roles.Contains("Landlord"))
            {
                var landlord = await _context.Landlords
                    .FirstOrDefaultAsync(x => x.ApplicationUserId == currentUser.Id, cancellationToken);

                if (landlord == null)
                    return Enumerable.Empty<PropertyDto>();

                properties = await query
                    .Where(x => x.LandlordId == landlord.LandlordId)
                    .ToListAsync(cancellationToken);

                return properties.Select(p => MapToDto(p));
            }

            // ========== TENANT ==========
            if (roles.Contains("Tenant"))
            {
                properties = await query
                    .Where(x => x.Status == Models.Enums.EntityStatus.Approved)
                    .ToListAsync(cancellationToken);

                return properties.Select(p => MapToDto(p));
            }

            // No valid role
            return Enumerable.Empty<PropertyDto>();
        }

        private static PropertyDto MapToDto(Models.Domain.Property p)
        {
            return new PropertyDto
            {
                PropertyId = p.PropertyId,
                PropertyName = p.PropertyName,
                Location = p.Location,
                Status = p.Status,
                LandlordName = p.Landlord?.ApplicationUser?.FirstName,
                TenantName = p.Tenant?.ApplicationUser?.FirstName
            };
        }
    }
}
