using HouseMaintenanceRequest.API.Models.DTOs.Account;
using MediatR;

namespace HouseMaintenanceRequest.API.Features.Account.RefreshToken.Command
{
    public record RefreshTokenCommand(string Email) : IRequest<ApplicationUserDto>;
}
