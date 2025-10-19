using HouseMaintenanceRequest.API.Models.DTOs.Account;
using MediatR;

namespace HouseMaintenanceRequest.API.Features.Account.Registration.Command
{
    public record RegisterCommand(RegisterDto RegisterDto) : IRequest<bool>;
}