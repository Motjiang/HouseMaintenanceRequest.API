using HouseMaintenanceRequest.API.Models.DTOs.Property_View;
using MediatR;

namespace HouseMaintenanceRequest.API.Features.Property_View.Query
{
    public record GetPropertyViewRequestsQuery()
        : IRequest<List<PropertyViewRequestResponseDto>>;
}
