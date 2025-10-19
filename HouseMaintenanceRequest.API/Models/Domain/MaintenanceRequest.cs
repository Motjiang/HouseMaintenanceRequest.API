using System.ComponentModel.DataAnnotations;
using HouseMaintenanceRequest.API.Models.Enums;

namespace HouseMaintenanceRequest.API.Models.Domain
{
    public class MaintenanceRequest
    {
        [Key]
        public int RequestId { get; set; }

        [Required]
        public string Description { get; set; }

        public DateTime DateRequested { get; set; }

        public EntityStatus Status { get; set; } 

        public bool IsDeleted { get; set; } = false;

        // Relationships
        public int TenantId { get; set; }
        public Tenant Tenant { get; set; }

        public int PropertyId { get; set; }
        public Property Property { get; set; }

        public int? MaintenanceCompanyId { get; set; }
        public MaintenanceCompany? MaintenanceCompany { get; set; }
    }
}
