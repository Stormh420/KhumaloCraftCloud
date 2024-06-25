using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace KhumaloCraftPOE.Functions
{
    public static class NotificationFunctions
    {
        [FunctionName("SendNotification")]
        public static async Task SendNotification([ActivityTrigger] NotificationInfo notificationInfo, ILogger log)
        {
            // Logic to send notification
            log.LogInformation($"Sending notification for order {notificationInfo.OrderId} at stage {notificationInfo.Stage}.");

            // Simulate sending notification
            await Task.Delay(500);

            log.LogInformation($"Notification for order {notificationInfo.OrderId} sent at stage {notificationInfo.Stage}.");
        }
    }

    public class NotificationInfo
    {
        public int OrderId { get; set; }
        public string? Stage { get; set; }
    }
}

