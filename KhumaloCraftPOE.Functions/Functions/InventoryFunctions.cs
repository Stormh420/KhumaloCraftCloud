using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace KhumaloCraftPOE.Functions
{
    public static class InventoryFunctions
    {
        [FunctionName("UpdateInventory")]
        public static async Task UpdateInventory([ActivityTrigger] int productId, ILogger log)
        {
            // Logic to update inventory
            log.LogInformation($"Updating inventory for product {productId}.");

            // Simulate inventory update
            await Task.Delay(1000);

            log.LogInformation($"Inventory for product {productId} updated.");
        }
    }
}

