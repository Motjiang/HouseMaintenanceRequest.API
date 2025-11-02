using HouseMaintenanceRequest.API.Data;
using HouseMaintenanceRequest.API.Features.Property_View.Query;
using HouseMaintenanceRequest.API.Models.Domain;
using HouseMaintenanceRequest.API.Models.DTOs.Property_View;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HouseMaintenanceRequest.API.Features.Property_View.Handler
{
    public class GetPropertyViewRequestsQueryHandler
            : IRequestHandler<GetPropertyViewRequestsQuery, List<PropertyViewRequestResponseDto>>
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetPropertyViewRequestsQueryHandler(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<PropertyViewRequestResponseDto>> Handle(
            GetPropertyViewRequestsQuery request,
            CancellationToken cancellationToken)
        {
            var currentUser = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);

            if (currentUser == null)
                throw new UnauthorizedAccessException("User not authenticated");

            var roles = await _userManager.GetRolesAsync(currentUser);

            var query = _context.PropertyViewRequests
                .Include(x => x.Property)
                    .ThenInclude(x => x.Landlord)
                        .ThenInclude(x => x.ApplicationUser)
                .Include(x => x.Tenant)
                    .ThenInclude(x => x.ApplicationUser)
                .AsQueryable();

            if (roles.Contains("Admin"))
            {
                // See all
            }
            else if (roles.Contains("Landlord"))
            {
                query = query.Where(x => x.Property.Landlord.ApplicationUserId == currentUser.Id);
            }
            else if (roles.Contains("Tenant"))
            {
                query = query.Where(x => x.Tenant.ApplicationUserId == currentUser.Id);
            }
            else
            {
                throw new UnauthorizedAccessException("Not authorized");
            }

            return await query
                .Select(x => new PropertyViewRequestResponseDto
                {
                    ViewRequestId = x.ViewRequestId,
                    PropertyId = x.PropertyId,
                    PropertyName = x.Property.PropertyName,
                    TenantId = x.TenantId,
                    TenantName = x.Tenant.ApplicationUser.FirstName,
                    RequestedAt = x.RequestedAt,
                    ScheduledAt = x.ScheduledAt,
                    Status = x.Status.ToString()
                })
                .ToListAsync(cancellationToken);
        }
    }
}
