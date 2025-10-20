using HouseMaintenanceRequest.API.Features.Account.LogIn.Command;
using HouseMaintenanceRequest.API.Models.Domain;
using HouseMaintenanceRequest.API.Models.DTOs.Account;
using HouseMaintenanceRequest.API.Services;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace HouseMaintenanceRequest.API.Features.Account.LogIn.Handler
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, ApplicationUserDto>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly JWTService _jwtService;
        private readonly ILogger<LoginCommandHandler> _logger;

        public LoginCommandHandler(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            JWTService jwtService,
            ILogger<LoginCommandHandler> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtService = jwtService;
            _logger = logger;
        }

        public async Task<ApplicationUserDto> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var model = request.LogInDto;
            var user = await _userManager.FindByNameAsync(model.UserName);

            if (user == null)
            {
                _logger.LogWarning("Invalid username attempt: {UserName}", model.UserName);
                throw new UnauthorizedAccessException("Invalid username or password");
            }

            if (!user.EmailConfirmed)
                throw new UnauthorizedAccessException("Please confirm your email before logging in.");

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);

            if (result.IsLockedOut)
            {
                throw new UnauthorizedAccessException(
                    $"Your account has been locked. You should wait until {user.LockoutEnd} (UTC time) to be able to login.");
            }

            if (!result.Succeeded)
            {
                // Increment AccessFailedCount
                await _userManager.AccessFailedAsync(user);

                if (user.AccessFailedCount >= Constants.MaxFailedAccessAttempts)
                {
                    // Lock user for one day
                    await _userManager.SetLockoutEndDateAsync(user, DateTime.Now.AddDays(1));
                    throw new UnauthorizedAccessException(
                        $"Your account has been locked. You should wait until {user.LockoutEnd} (UTC time) to be able to login.");
                }

                throw new UnauthorizedAccessException("Invalid username or password");
            }

            // Reset failed count and unlock account
            await _userManager.ResetAccessFailedCountAsync(user);
            await _userManager.SetLockoutEndDateAsync(user, null);

            // Generate JWT and map user
            var jwt = await _jwtService.CreateJWT(user);

            _logger.LogInformation("✅ {Email} logged in successfully", user.Email);

            return new ApplicationUserDto
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                JWT = jwt
            };
        }
    }
}
