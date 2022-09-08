using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ServerlessOpenHackAPI.Services;

namespace ServerlessOpenHackAPI
{
    public static class GetProduct
    {
        public static IProductService productService = new ProductService();

        [FunctionName("GetProduct")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Serverless OH API - Get Product Request Initiated");

            string productId = req.Query["productId"];
            Guid guidProductId;

            log.LogInformation($"ProductId Captured: {productId}");

            if (productId != null && Guid.TryParse(productId, out guidProductId))
            {
                var product = await productService.GetProduct(guidProductId);
                return product != null
                            ? (ActionResult)new JsonResult(product)
                            : new BadRequestObjectResult("Product does not exist");
            }
            else
            {
                return new BadRequestObjectResult("Please pass a valid productId on the query string");
            }
        }
    }
}
