using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace HouseMaintenanceRequest.API.Models.Domain
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;
        [Required]
        public string LastName { get; set; } = string.Empty;
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;

        public int? TenantId { get; set; }
        public Tenant? Tenant { get; set; }

        public int? LandlordId { get; set; }
        public Landlord? Landlord { get; set; }

        public int? MaintenanceCompanyId { get; set; }
        public MaintenanceCompany? MaintenanceCompany { get; set; }
    }
}
