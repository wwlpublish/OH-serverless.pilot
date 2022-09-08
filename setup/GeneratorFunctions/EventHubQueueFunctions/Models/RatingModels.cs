namespace ServerlessOpenhack.Models
{
    public class RatingDto
    {
        public string userId { get; set; }
        public string productId { get; set; }
        public string locationName { get; set; }
        public int rating { get; set; }
        public string userNotes { get; set; }
    }

    public class RatingDetails
    {
        public string RatingEndpoint { get; set; }
        public RatingDto Rating { get; set; }
    }
}