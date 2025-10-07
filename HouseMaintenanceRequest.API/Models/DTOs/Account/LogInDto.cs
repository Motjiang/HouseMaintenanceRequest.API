using System.ComponentModel.DataAnnotations;

namespace HouseMaintenanceRequest.API.Models.DTOs.Account
{
    public class LogInDto
    {
        [Required(ErrorMessage = "Username is required")]
        [RegularExpression("^\\w+@[a-zA-Z_]+?\\.[a-zA-Z]{2,3}$", ErrorMessage = "Invalid email address")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }
}
