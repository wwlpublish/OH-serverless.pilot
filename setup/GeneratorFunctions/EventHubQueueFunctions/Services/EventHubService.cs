using Microsoft.Azure.WebJobs.Host;
using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using ServerlessOpenhack.Models;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Text;
using System.Threading;
using System.Linq;

namespace ServerlessOpenhack.Services
{
    public class EventHubService
    {
        private List<EventHubTeamClientEntry> teamClientEntries;
        public static EventHubService hubClientProvider;

        // CONSTRUCTOR
        public EventHubService() {
            this.teamClientEntries = new List<EventHubTeamClientEntry>();
        }

        public static CloudQueue GetTriggerQueueReference()
        {
            // Retrieve storage account from connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                Microsoft.Azure.CloudConfigurationManager.GetSetting("AzureWebJobsStorage"));

            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();

            // Retrieve a reference to a container.
            return queueClient.GetQueueReference("event-hubs-trigger-queue");
        }

        private EventHubClient ProvisionNewEventHubClient(string teamTableNumber, EventHubConnectionInfo connectionInfo)
        {
            var eventHubClient = EventHubClient.CreateFromConnectionString(connectionInfo.eventHubConnectionString, connectionInfo.eventHubName);

            // replace old entry
            teamClientEntries.Add(new EventHubTeamClientEntry
            {
                teamTableNumber = teamTableNumber,
                eventHubClient = eventHubClient,
                eventHubConnectionInfo = new EventHubConnectionInfo() {
                    eventHubConnectionString = connectionInfo.eventHubConnectionString,
                    eventHubName = connectionInfo.eventHubName
                }
            });
            return eventHubClient;
        }

        // get event hub from client store if it's been provisioned before
        public EventHubClient GetEventHubClient(EventHubTeamInfo eventHubTeamInfo, TraceWriter log)
        {
            int matchIndex;
            try
            {
                matchIndex = teamClientEntries.FindIndex(entry => eventHubTeamInfo.teamTableNumber == entry.teamTableNumber);

                EventHubConnectionInfo messageConnectionInfo = new EventHubConnectionInfo()
                {
                    eventHubConnectionString = eventHubTeamInfo.eventHubConnectionString,
                    eventHubName = eventHubTeamInfo.eventHubName
                };

                // no existing eventHubClient
                if (matchIndex == -1)
                {
                    return ProvisionNewEventHubClient(eventHubTeamInfo.teamTableNumber, messageConnectionInfo);
                }
                else
                {
                    // if you're returning existing client, check if it is correct
                    var matchConnectionInfo = teamClientEntries[matchIndex].eventHubConnectionInfo;
                    var isSameInstance = matchConnectionInfo.eventHubConnectionString == eventHubTeamInfo.eventHubConnectionString
                        && matchConnectionInfo.eventHubName == eventHubTeamInfo.eventHubName;

                    return isSameInstance
                        ? teamClientEntries[matchIndex].eventHubClient
                        : ProvisionNewEventHubClient(teamClientEntries[matchIndex].teamTableNumber, messageConnectionInfo);
                }
            }
            catch (Exception e)
            {
                log.Info($"{e.Message}  Check registration for {eventHubTeamInfo.teamTableNumber}?");
                return null;
            }
        }

        public async static Task SendEventToHub(EventHubTeamInfo eventHubTeamInfo, TraceWriter log)
        {
            try
            {
                hubClientProvider = hubClientProvider ?? new EventHubService();
                var eventHubClient = hubClientProvider.GetEventHubClient(eventHubTeamInfo, log);

                if (eventHubTeamInfo.eventHubBoost) // boost batch
                {
                    var messagesPerThread = 250;
                    var boostThreadsToStart = 5;
                    var totalMessages = messagesPerThread * boostThreadsToStart;

                    var eventHubTaskList = new List<Task>();

                    while (boostThreadsToStart > 0)
                    {
                        eventHubTaskList.Add(Task.Run(async () =>
                        {
                            // INSIDE NEW THREAD
                            var eventDataList = RecordGenerationService.GenerateEvents(messagesPerThread);

                            long maxNumberBytes = 256000;
                            long currentBatchSize = 0;
                            var eventDataBatch = new List<EventData>();

                            for (int i = 0; i < messagesPerThread; i++)
                            {
                                var singleEvent = JsonConvert.SerializeObject(eventDataList[i],
                                    new IsoDateTimeConverter() { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" });

                                var encodedMessage = new EventData(Encoding.UTF8.GetBytes(singleEvent));

                                // batch not too big
                                if (encodedMessage.SerializedSizeInBytes + currentBatchSize < maxNumberBytes)
                                {
                                    eventDataBatch.Add(encodedMessage);
                                    currentBatchSize = currentBatchSize + encodedMessage.SerializedSizeInBytes;
                                }
                                //batch too big to add
                                else
                                {
                                    // send what's in the batch
                                    try
                                    {
                                        await eventHubClient.SendBatchAsync(eventDataBatch);
                                        //log.Info($"Thread {Thread.CurrentThread.ManagedThreadId} sent up to {i} events");
                                    }
                                    catch (Exception exception)
                                    {
                                        log.Info($"{DateTime.Now} > Exception: {exception.Message}");
                                    }

                                    // reset batch
                                    currentBatchSize = 0;
                                    eventDataBatch = new List<EventData>()
                                    {
                                        encodedMessage
                                    };
                                }
                            }

                            // when you have no more messages to add to the batch, send what's still in the batch
                            if (eventDataBatch.Count != 0)
                            {
                                try
                                {
                                    await eventHubClient.SendBatchAsync(eventDataBatch);
                                    log.Info($"Thread {Thread.CurrentThread.ManagedThreadId} sent up to {messagesPerThread} events");
                                }
                                catch (Exception exception)
                                {
                                    log.Info($"{DateTime.Now} > Exception: {exception.Message}");
                                }
                            }
                        }));

                        boostThreadsToStart--;
                    }

                    await Task.WhenAll(eventHubTaskList);

                    log.Info($"{totalMessages} messages sent.");
                }
                else // standard batch
                {
                    var standardBatchSize = 3;

                    var eventDataBatch = RecordGenerationService
                        .GenerateEvents(standardBatchSize)
                        .Select(generatedMessage =>
                        {
                            var singleEvent = JsonConvert.SerializeObject(generatedMessage,
                                new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" });

                            var encodedMessage = new EventData(Encoding.UTF8.GetBytes(singleEvent));
                            return encodedMessage;
                        });

                    try
                    {
                        await eventHubClient.SendBatchAsync(eventDataBatch);
                        log.Info($"{standardBatchSize} messages sent.");
                    }
                    catch (Exception exception)
                    {
                        log.Info($"{DateTime.Now} > Exception: {exception.Message}");
                    }
                }
            }
            catch (Exception e)
            {
                log.Info(e.Message);
            }
        }
    }    
}