using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace KhumaloCraftPOE.Functions
{
    public static class PaymentFunctions
    {
        [FunctionName("ProcessPayment")]
        public static async Task ProcessPayment([ActivityTrigger] PaymentInfo paymentInfo, ILogger log)
        {
            // Logic to process payment
            log.LogInformation($"Processing payment for order {paymentInfo.OrderId}.");

            // Simulate payment processing
            await Task.Delay(1000);

            log.LogInformation($"Payment for order {paymentInfo.OrderId} processed.");
        }
    }

    public class PaymentInfo
    {
        public int OrderId { get; set; }
        public decimal Amount { get; set; }
        public string? PaymentMethod { get; set; }
    }
}

