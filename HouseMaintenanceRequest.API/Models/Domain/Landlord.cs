using HouseMaintenanceRequest.API.Models.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;

namespace HouseMaintenanceRequest.API.Models.Domain
{
    public class Landlord : NavigationProperties
    {
        [Key]
        public int LandlordId { get; set; }

        [Required]
        public string PhysicalAddress { get; set; }

        public string? BusinessDocumentPath { get; set; }

        public EntityStatus Status { get; set; } 

        public bool IsDeleted { get; set; } = false;

        // Relationships
        public ICollection<Property> Properties { get; set; } = new List<Property>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
}
