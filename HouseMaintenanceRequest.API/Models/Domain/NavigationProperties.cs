namespace HouseMaintenanceRequest.API.Models.Domain
{
    public abstract class NavigationProperties
    {
        // Navigation to User
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}
