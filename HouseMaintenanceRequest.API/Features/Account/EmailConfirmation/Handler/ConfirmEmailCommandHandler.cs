using HouseMaintenanceRequest.API.Features.Account.EmailConfirmation.Command;
using HouseMaintenanceRequest.API.Models.Domain;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace HouseMaintenanceRequest.API.Features.Account.EmailConfirmation.Handler
{
    public class ConfirmEmailCommandHandler : IRequestHandler<ConfirmEmailCommand, bool>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ConfirmEmailCommandHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<bool> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
        {
            var dto = request.ConfirmEmailDto;

            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                throw new UnauthorizedAccessException("This email address has not been registered yet");

            if (user.EmailConfirmed)
                throw new InvalidOperationException("Email already confirmed.");

            try
            {
                var decodedTokenBytes = WebEncoders.Base64UrlDecode(dto.Token);
                var decodedToken = Encoding.UTF8.GetString(decodedTokenBytes);

                var result = await _userManager.ConfirmEmailAsync(user, decodedToken);
                if (!result.Succeeded)
                    throw new InvalidOperationException("Invalid token. Please try again");

                return true;
            }
            catch
            {
                throw new InvalidOperationException("Invalid token. Please try again");
            }
        }
    }
}
