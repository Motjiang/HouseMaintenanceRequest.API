using System.ComponentModel.DataAnnotations;
using HouseMaintenanceRequest.API.Models.Enums;

namespace HouseMaintenanceRequest.API.Models.Domain
{
    public class Property
    {
        [Key]
        public int PropertyId { get; set; }

        [Required]
        public string PropertyName { get; set; }

        [Required]
        public string Location { get; set; }

        public EntityStatus Status { get; set; } 
        public bool IsDeleted { get; set; } = false;

        // Foreign Keys
        public int LandlordId { get; set; }
        public Landlord Landlord { get; set; }

        public int? TenantId { get; set; }
        public Tenant? Tenant { get; set; }

        public ICollection<MaintenanceRequest> MaintenanceRequests { get; set; } = new List<MaintenanceRequest>();
    }
}
