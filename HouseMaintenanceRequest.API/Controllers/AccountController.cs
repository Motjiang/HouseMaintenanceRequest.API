using HouseMaintenanceRequest.API.Features.Account.EmailConfirmation.Command;
using HouseMaintenanceRequest.API.Features.Account.LogIn.Command;
using HouseMaintenanceRequest.API.Features.Account.Registration.Command;
using HouseMaintenanceRequest.API.Models.Domain;
using HouseMaintenanceRequest.API.Models.DTOs.Account;
using HouseMaintenanceRequest.API.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;

namespace HouseMaintenanceRequest.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMediator _mediator;
        private readonly JWTService _jwtService;

        public AccountController(UserManager<ApplicationUser> userManager, IMediator mediator, JWTService jwtService)
        {
            _userManager = userManager;
            _mediator = mediator;
            _jwtService = jwtService;
        }

        [Authorize]
        [HttpGet("refresh-user-token")]
        public async Task<ActionResult<ApplicationUserDto>> RefreshUserToken()
        {
            var user = await _userManager.FindByNameAsync(User.FindFirst(ClaimTypes.Email)?.Value);

            if (await _userManager.IsLockedOutAsync(user))
                return Unauthorized("You have been locked out");

            return  new ApplicationUserDto
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                JWT = await _jwtService.CreateJWT(user),
            };
        }

        // ✅ Register
        [HttpPost("register")]
        [EnableRateLimiting("account_crud")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _mediator.Send(new RegisterCommand(dto));

                if (!result)
                    return Conflict("User already exists or registration failed.");

                return Ok(new { message = "Account created successfully. Please check your email to confirm." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Registration error: {ex.Message}");
                return StatusCode(500, "An error occurred while creating the account.");
            }
        }

        // ✅ Login
        [HttpPost("login")]
        [EnableRateLimiting("account_crud")]
        public async Task<ActionResult<ApplicationUserDto>> Login([FromBody] LogInDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var userDto = await _mediator.Send(new LoginCommand(dto));
                return Ok(userDto);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Login error: {ex.Message}");
                return StatusCode(500, "An error occurred while logging in.");
            }
        }

        

        // ✅ Confirm Email
        [HttpPut("confirm-email")]
        [EnableRateLimiting("account_crud")]
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var success = await _mediator.Send(new ConfirmEmailCommand(model));
                if (success)
                    return Ok(new { title = "Email confirmed", message = "Your email address is confirmed. You can login now." });

                return BadRequest("Email confirmation failed.");
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Confirm email error: {ex.Message}");
                return StatusCode(500, "An error occurred while confirming email.");
            }
        }
    }
}
