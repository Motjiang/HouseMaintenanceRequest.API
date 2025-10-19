using HouseMaintenanceRequest.API.Models.Enums;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace HouseMaintenanceRequest.API.Models.DTOs.Account
{
    public class RegisterDto : IRequest<bool>
    {
        [Required(ErrorMessage = "First name is required.")]
        public string FirstName { get; set; } 

        [Required(ErrorMessage = "Last name is required.")]
        public string LastName { get; set; } 

        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        public string Email { get; set; } 

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string Password { get; set; } 

        [Required(ErrorMessage = "Role is required.")]
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