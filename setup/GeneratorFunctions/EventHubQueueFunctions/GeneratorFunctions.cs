using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using ServerlessOpenhack.Models;
using ServerlessOpenhack.Services;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using System.Linq;
using Microsoft.WindowsAzure.Storage.Queue;

namespace ServerlessOpenhack.Functions
{
    public static class GeneratorFunctions
    {
        static HttpClient client = new HttpClient();
        static CloudQueue queue;

        [FunctionName("GenerateRating")]
        public static async Task GenerateRating([TimerTrigger("*/4 * * * * *")]TimerInfo myTimer, TraceWriter log)
        {
            log.Info($"C# Timer trigger GenerateRating function executed at: {DateTime.Now}");

            var ratingTaskList = new List<Task<HttpResponseMessage>>();

            foreach (OpenHackTeam team in await CosmosService.GetRegisteredRatingsTeams(log))
            {
                ratingTaskList.Add(client.PostAsJsonAsync(team.ratingEndpoint, RecordGenerationService.GenerateRating()));
            }

            await Task.WhenAll(ratingTaskList);

            log.Info("Ratings generation finished.");
        }

        [FunctionName("RelayRatingUI")]
        public static async Task<HttpResponseMessage> RelayRatingUI([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function RelayRatingUI processed a request.");
            
            // Get request body
            var data = await req.Content.ReadAsStringAsync();
            RatingDetails ratingDetails = JsonConvert.DeserializeObject<RatingDetails>(data);

            var response = await client.PostAsJsonAsync<RatingDto>(ratingDetails.RatingEndpoint, ratingDetails.Rating);
            var responseJsonString = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject(responseJsonString);

            return req.CreateResponse(response.StatusCode, responseObject);
        }

        [FunctionName("SendCSV")]
        public static async Task SendCSV([TimerTrigger("0 * * * * *")]TimerInfo myTimer, TraceWriter log)
        {
            log.Info($"C# Timer trigger SendCSV function executed at: {DateTime.Now}");

            //Generate orders
            Random r = new Random();
            int numOrders = r.Next(1, 8);
            List<Order> orders = RecordGenerationService.GenerateCSVs(numOrders);

            //Generate the CSV content
            string firstCSV = CSVService.CreateFirstCSVContent(orders);
            string secondCSV = CSVService.CreateSecondCSVContent(orders);
            string thirdCSV = CSVService.CreateThirdCSVContent(orders);

            var fileBatchTaskList = new List<Task>();

            //Send the content to each team
            foreach (var team in await CosmosService.GetRegisteredStorageAccountTeams(log))
            {
                fileBatchTaskList.Add(CSVService.SendBatch(team, firstCSV, secondCSV, thirdCSV, log));
            }

            await Task.WhenAll(fileBatchTaskList);
        }

        [FunctionName("EventHubQueuePublisher")]
        public static async Task EventHubQueuePublisher([TimerTrigger("*/5 * * * * *")]TimerInfo myTimer, TraceWriter log)
        {
            // publishes event hub info to storage account queue for each team on that has registered an Event Hub endpoint
            log.Info($"C# Timer trigger EventHubQueuePublisher function executed at: {DateTime.Now}");

            queue = queue ?? EventHubService.GetTriggerQueueReference();

            // Create the queue if it doesn't already exist
            await queue.CreateIfNotExistsAsync();

            var queueTaskList = new List<Task>();

            foreach (var team in await CosmosService.GetRegisteredEventHubTeams(log))
            {
                var queueMessage = new CloudQueueMessage(
                    JsonConvert.SerializeObject(new EventHubTeamInfo()
                    {
                        teamTableNumber = team.teamTableNumber,
                        eventHubConnectionString = team.eventHubConnectionString,
                        eventHubName = team.eventHubName,
                        eventHubBoost = team.eventHubBoost
                    }));

                queueTaskList.Add(queue.AddMessageAsync(queueMessage));
            }

            await Task.WhenAll(queueTaskList);

            log.Info($"Queue publisher successfully published to storage queue.");
        }

        [FunctionName("EventHubQueueSubscriber")]
        public async static Task EventHubQueueSubscriber([QueueTrigger("event-hubs-trigger-queue")]string myQueueItem, TraceWriter log)
        {
            // sends messages to event hub for each team whose info is being sent to the storage account queue via EventHubQueuePublisher function
            var eventHubTeamInfo = JsonConvert.DeserializeObject<EventHubTeamInfo>(myQueueItem);

            await EventHubService.SendEventToHub(eventHubTeamInfo, log);

            log.Info($"C# Queue trigger function processed: {myQueueItem}");
        }
    }
}
