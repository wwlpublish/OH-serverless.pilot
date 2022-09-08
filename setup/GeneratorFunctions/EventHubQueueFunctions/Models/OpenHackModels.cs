using System;

namespace ServerlessOpenhack.Models
{
    public class OpenHackTeam
    {
        public string teamTableNumber { get; set; }
        public DateTime dateRegistered { get; set; }
        public string eventHubConnectionString { get; set; }
        public string eventHubName { get; set; }
        public string storageAccountConnectionString { get; set; }
        public string blobContainerName { get; set; }
        public string ratingEndpoint { get; set; }
        public bool registeredStorageAccount { get; set; }
        public bool registeredEventHub { get; set; }
        public bool registeredRatings { get; set; }
        public bool eventHubBoost { get; set; }
        public string contentUrl { get; set; }
    };
}
