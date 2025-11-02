using HouseMaintenanceRequest.API.Models.DTOs.Property_View;
using MediatR;

namespace HouseMaintenanceRequest.API.Features.Property_View.Command
{
    public record CreatePropertyViewRequestCommand(
        int tenantId,
        int propertyId,
        DateTime scheduledAt
    ) : IRequest<int>;
}

