using HouseMaintenanceRequest.API.Data;
using HouseMaintenanceRequest.API.Models.Domain;
using HouseMaintenanceRequest.API.Models.Enums;
using HouseMaintenanceRequest.API.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace HouseMaintenanceRequest.API.System_Communication.Hubs
{
    public class NotificationHub : Hub
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public NotificationHub(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // ✅ Send notification to a specific user
        public async Task SendNotificationToUser(
            string userId,
            string title,
            string message,
            string targetController = "Home",
            string targetAction = "Index")
        {
            var notification = new Notification
            {
                Title = title,
                Message = message,
                RecipientUserId = userId,
                CreatedAt = DateTime.Now,
                TargetController = targetController,
                TargetAction = targetAction,
                IsRead = false
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            await Clients.User(userId).SendAsync("ReceiveNotification", notification);
        }

        // ✅ Notify all Admins
        public async Task NotifyAdmins(string title, string message)
        {
            var admins = await _userManager.GetUsersInRoleAsync(Constants.Role_Admin);

            foreach (var admin in admins)
            {
                var notification = new Notification
                {
                    Title = title,
                    Message = message,
                    RecipientUserId = admin.Id,
                    CreatedAt = DateTime.Now,
                    TargetController = "Admin",
                    TargetAction = "PendingApprovals",
                    IsRead = false
                };

                _context.Notifications.Add(notification);
                await Clients.User(admin.Id).SendAsync("ReceiveNotification", notification);
            }

            await _context.SaveChangesAsync();
        }
    }
}
