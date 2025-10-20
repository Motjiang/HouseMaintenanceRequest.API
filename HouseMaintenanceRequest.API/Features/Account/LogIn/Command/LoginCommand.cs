using HouseMaintenanceRequest.API.Models.DTOs.Account;
using MediatR;

namespace HouseMaintenanceRequest.API.Features.Account.LogIn.Command
{
    public record LoginCommand(LogInDto LogInDto) : IRequest<ApplicationUserDto>;
}
