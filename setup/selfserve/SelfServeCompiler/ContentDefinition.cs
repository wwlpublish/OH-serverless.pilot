using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SelfServeCompiler
{
    public class ContentItem
    {
        [JsonPropertyName("Order")]
        public int Order { get; set; }

        [JsonPropertyName("Title")]
        public string Title { get; set; }

        [JsonPropertyName("Description")]
        public string Description { get; set; }

        [JsonPropertyName("Url")]
        public string Url { get; set; }

        [JsonPropertyName("Follows")]
        public int? Follows { get; set; }
    }

    public class ContentPackage
    {
        [JsonPropertyName("ContentItems")]
        public List<ContentItem> ContentItems { get; set; }
    }

    public class ContentDefinition
    {
        [JsonPropertyName("Title")]
        public string Title { get; set; }

        [JsonPropertyName("Description")]
        public string Description { get; set; }

        [JsonPropertyName("OverviewUrl")]
        public string OverviewUrl { get; set; }

        [JsonPropertyName("ContentVersion")]
        public string ContentVersion { get; set; }

        [JsonPropertyName("ContentPackage")]
        public ContentPackage ContentPackage { get; set; }
    }


}
