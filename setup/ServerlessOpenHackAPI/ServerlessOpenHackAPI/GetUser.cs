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
using ServerlessOpenHackAPI.Models;

namespace ServerlessOpenHackAPI
{
    public static class GetUser
    {
        public static IUserService userService = new UserService();

        [FunctionName("GetUser")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Serverless OH API - Get User Request Initiated");

            string userid = req.Query["userId"];
            Guid guiduserid;

            log.LogInformation($"UserId Captured: {userid}");

            if (userid != null && Guid.TryParse(userid, out guiduserid))
            {
                var user = await userService.GetUser(guiduserid);
                return user != null
                            ? (ActionResult)new JsonResult(user)
                            : new BadRequestObjectResult("User does not exist");
            }
            else
            {
                return new BadRequestObjectResult("Please pass a valid userId on the query string");
            }
        }
    }
}
