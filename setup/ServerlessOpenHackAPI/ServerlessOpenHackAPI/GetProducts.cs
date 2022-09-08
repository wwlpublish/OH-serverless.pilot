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
    
    public static class GetProducts
    {
        public static IProductService productService = new ProductService();

        [FunctionName("GetProducts")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Serverless OH API - Get All Products Request Initiated");

            var result = await productService.ListProducts();

            return new OkObjectResult(result);
        }
    }
}
