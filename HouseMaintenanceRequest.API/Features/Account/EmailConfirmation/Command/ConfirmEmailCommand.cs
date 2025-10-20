using HouseMaintenanceRequest.API.Models.DTOs.Account;
using MediatR;

namespace HouseMaintenanceRequest.API.Features.Account.EmailConfirmation.Command
{
    public record ConfirmEmailCommand(ConfirmEmailDto ConfirmEmailDto) : IRequest<bool>;
}
