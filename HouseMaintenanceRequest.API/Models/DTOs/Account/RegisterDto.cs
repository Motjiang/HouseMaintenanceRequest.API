using HouseMaintenanceRequest.API.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace HouseMaintenanceRequest.API.Models.DTOs.Account
{
    public class RegisterDto
    {
        [Required]
        public string FirstName { get; set; } 

        [Required]
        public string LastName { get; set; } 

        [Required]
        [EmailAddress]
        public string Email { get; set; } 

        [Required]
        [MinLength(6)]
        public string Password { get; set; } 

        [Required]
        public string Role { get; set; } 

        // Landlord-specific fields
        public string? PhysicalAddress { get; set; }
        public string? BusinessDocumentPath { get; set; }

        // MaintenanceCompany-specific fields
        public string? CompanyName { get; set; }
        public MaintenanceType? TypeOfMaintenance { get; set; }
        public string? DocumentPath { get; set; }
    }
}