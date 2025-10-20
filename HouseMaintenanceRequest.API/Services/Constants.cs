namespace HouseMaintenanceRequest.API.Services
{
    public static class Constants
    {
        public const string Role_Admin = "Admin";
        public const string Role_Landlord = "Landlord";
        public const string Role_Tenant = "Tenant";
        public const string Role_MaintenanceCompany = "MaintenanceCompany";

        // Login attempt limits
        public const int MaxFailedAccessAttempts = 3;
    }
}
