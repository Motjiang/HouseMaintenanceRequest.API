using HouseMaintenanceRequest.API.Models.Enums;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;

namespace HouseMaintenanceRequest.API.Models.Domain
{
    public class Tenant : NavigationProperties
    {
        [Key]
        public int TenantId { get; set; }

        public EntityStatus Status { get; set; }
        public bool IsDeleted { get; set; } = false;

        // Relationships
        public int? PropertyId { get; set; }
        public Property? Property { get; set; }

        public ICollection<MaintenanceRequest> MaintenanceRequests { get; set; } = new List<MaintenanceRequest>();
        public ICollection<PropertyViewRequest> ViewRequests { get; set; } = new List<PropertyViewRequest>();
    }
}
