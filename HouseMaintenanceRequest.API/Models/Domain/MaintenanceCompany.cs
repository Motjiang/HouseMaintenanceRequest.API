using System.ComponentModel.DataAnnotations;
using HouseMaintenanceRequest.API.Models.Enums;

namespace HouseMaintenanceRequest.API.Models.Domain
{
    public class MaintenanceCompany : NavigationProperties
    {
        [Key]
        public int MaintenanceCompanyId { get; set; }

        [Required]
        public string CompanyName { get; set; }

        [Required]
        public MaintenanceType TypeOfMaintenance { get; set; }

        public string? DocumentPath { get; set; }

        public EntityStatus Status { get; set; } 

        public bool IsDeleted { get; set; } = false;

        public ICollection<MaintenanceRequest> MaintenanceRequests { get; set; } = new List<MaintenanceRequest>();
    }
}
