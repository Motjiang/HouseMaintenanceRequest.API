using System.ComponentModel.DataAnnotations;
using HouseMaintenanceRequest.API.Models.Enums;

namespace HouseMaintenanceRequest.API.Models.Domain
{
    public class PropertyViewRequest
    {
        [Key]
        public int ViewRequestId { get; set; }

        public int TenantId { get; set; }
        public Tenant Tenant { get; set; }

        public int PropertyId { get; set; }
        public Property Property { get; set; }

        public DateTime RequestedAt { get; set; } 

        public DateTime? ScheduledAt { get; set; }

        public ViewRequestStatus Status { get; set; } 
    }
}
