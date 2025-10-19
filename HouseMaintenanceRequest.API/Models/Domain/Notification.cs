using System.ComponentModel.DataAnnotations;

namespace HouseMaintenanceRequest.API.Models.Domain
{
    public class Notification
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Message { get; set; }

        public bool IsRead { get; set; } = false;

        public DateTime CreatedAt { get; set; } 

        [Required]
        public string RecipientUserId { get; set; }
        public ApplicationUser RecipientUser { get; set; }

        public string TargetController { get; set; } = "Home";
        public string TargetAction { get; set; } = "Index";
    }
}
