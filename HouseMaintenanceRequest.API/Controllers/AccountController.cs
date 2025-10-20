using HouseMaintenanceRequest.API.Features.Account.EmailConfirmation.Command;
using HouseMaintenanceRequest.API.Features.Account.LogIn.Command;
using HouseMaintenanceRequest.API.Features.Account.RefreshToken.Command;
using HouseMaintenanceRequest.API.Features.Account.Registration.Command;
using HouseMaintenanceRequest.API.Models.DTOs.Account;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace HouseMaintenanceRequest.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AccountController(IMediator mediator)
        {
            _mediator = mediator;
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

        // ✅ Refresh JWT Token
        [Authorize]
        [HttpGet("refresh-user-token")]
        public async Task<ActionResult<ApplicationUserDto>> RefreshUserToken()
        {
            try
            {
                var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
                if (string.IsNullOrEmpty(email))
                    return Unauthorized("Invalid user.");

                var userDto = await _mediator.Send(new RefreshTokenCommand(email));
                return Ok(userDto);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Token refresh error: {ex.Message}");
                return StatusCode(500, "An error occurred while refreshing token.");
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
