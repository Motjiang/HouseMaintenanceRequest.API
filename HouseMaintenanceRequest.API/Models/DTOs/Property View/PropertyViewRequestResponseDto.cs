namespace HouseMaintenanceRequest.API.Models.DTOs.Property_View
{
    public class PropertyViewRequestResponseDto
    {
        public int ViewRequestId { get; set; }
        public int TenantId { get; set; }
        public string TenantName { get; set; }

        public int PropertyId { get; set; }
        public string PropertyName { get; set; }

        public DateTime RequestedAt { get; set; }
        public DateTime? ScheduledAt { get; set; }

        public string Status { get; set; }
    }
}
