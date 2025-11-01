using HouseMaintenanceRequest.API.Models.DTOs.Property;
using MediatR;

namespace HouseMaintenanceRequest.API.Features.Property.Query
{
    public record GetPropertiesQuery() : IRequest<IEnumerable<PropertyDto>>;
}
