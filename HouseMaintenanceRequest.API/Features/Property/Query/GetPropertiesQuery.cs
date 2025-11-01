using HouseMaintenanceRequest.API.Models.DTOs.Property;
using MediatR;

namespace HouseMaintenanceRequest.API.Features.Property.Query
{
    public record GetPropertiesQuery(int PageNumber, int PageSize) : IRequest<object>;
}