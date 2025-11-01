using HouseMaintenanceRequest.API.Models.Enums;

namespace HouseMaintenanceRequest.API.Models.DTOs.Property
{
    public class PropertyDto
    {
        public int PropertyId { get; set; }
        public string PropertyName { get; set; }
        public string Location { get; set; }
        public EntityStatus Status { get; set; }
        public string LandlordName { get; set; }
        public string TenantName { get; set; }
    }
}