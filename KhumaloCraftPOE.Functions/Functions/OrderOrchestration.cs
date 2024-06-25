using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace KhumaloCraftPOE.Functions
{
    public static class OrderOrchestration
    {
        [FunctionName("OrderOrchestrator")]
        public static async Task RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context, ILogger log)
        {
            int? orderId = context.GetInput<int?>();

            // Update Inventory
            await context.CallActivityAsync("UpdateInventory", orderId);

            // Process Payment
            var paymentInfo = new PaymentInfo
            {
                OrderId = orderId.GetValueOrDefault(),
                Amount = 100.0m, // This should come from your actual order data
                PaymentMethod = "CreditCard" // This should come from your actual order data
            };
            await context.CallActivityAsync("ProcessPayment", paymentInfo);

            // Send Order Confirmation Notification
            var notificationInfo = new NotificationInfo
            {
                OrderId = orderId.GetValueOrDefault(),
                Stage = "OrderConfirmation"
            };
            await context.CallActivityAsync("SendNotification", notificationInfo);

            // You can add more steps as required
        }

        [FunctionName("OrderOrchestration_HttpStart")]
        public static async Task<HttpResponseMessage> HttpStart(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestMessage req,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            // Function input comes from the request content.
            int? orderId = await req.Content.ReadAsAsync<int?>();
            if (!orderId.HasValue)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("Please provide an orderId in the request body.")
                };
            }

            // Convert int? to string for passing as a reference type
            string? orderIdString = orderId?.ToString();

            string instanceId = await starter.StartNewAsync("OrderOrchestrator", orderIdString);

            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

            return starter.CreateCheckStatusResponse(req, instanceId);
        }
    }
}




