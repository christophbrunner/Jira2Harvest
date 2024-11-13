namespace Jira2Harvest
{
    public class HarvestSettings
    {
        public required string AccessToken { get; set; }
        public required int HarvestAccountId { get; set; }
        public required long TaskId { get; set; }
    }
}