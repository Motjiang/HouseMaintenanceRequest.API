using HouseMaintenanceRequest.API.Models.Enums;
using MediatR;

namespace HouseMaintenanceRequest.API.Features.Property.Command
{
    public record CreatePropertyCommand(string propertyName, string location, int landLordId) : IRequest<int>;    
}
