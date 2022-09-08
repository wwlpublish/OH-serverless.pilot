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
    public static class GetUsers
    {
        public static IUserService userService = new UserService();

        [FunctionName("GetUsers")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Serverless OH API - Get All Users Request Initiated");

            var result = await userService.ListUsers();
            
            return new OkObjectResult(result);
        }
    }
}
