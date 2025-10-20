using HouseMaintenanceRequest.API.Features.Account.RefreshToken.Command;
using HouseMaintenanceRequest.API.Models.Domain;
using HouseMaintenanceRequest.API.Models.DTOs.Account;
using HouseMaintenanceRequest.API.Services;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace HouseMaintenanceRequest.API.Features.Account.RefreshToken.Handler
{
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, ApplicationUserDto>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JWTService _jwtService;

        public RefreshTokenCommandHandler(UserManager<ApplicationUser> userManager, JWTService jwtService)
        {
            _userManager = userManager;
            _jwtService = jwtService;
        }

        public async Task<ApplicationUserDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
                throw new UnauthorizedAccessException("User not found.");

            if (await _userManager.IsLockedOutAsync(user))
                throw new UnauthorizedAccessException("User is locked out.");

            return new ApplicationUserDto
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                JWT = await _jwtService.CreateJWT(user)
            };
        }
    }
}
