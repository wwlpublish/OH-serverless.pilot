using Microsoft.ServiceBus.Messaging;

namespace ServerlessOpenhack.Models
{
    public class EventHubTeamInfo
    {
        public string teamTableNumber { get; set; }
        public string eventHubConnectionString { get; set; }
        public string eventHubName { get; set; }
        public bool eventHubBoost { get; set; }
    }

    public class EventHubTeamClientEntry
    {
        public string teamTableNumber { get; set; }
        public EventHubClient eventHubClient { get; set; }
        public EventHubConnectionInfo eventHubConnectionInfo { get; set; }
    }

    public class EventHubConnectionInfo
    {
        public string eventHubConnectionString { get; set; }
        public string eventHubName { get; set; }
    }
}
