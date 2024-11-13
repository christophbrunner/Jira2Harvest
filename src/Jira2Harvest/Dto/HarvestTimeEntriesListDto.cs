using System.Text.Json.Serialization;

namespace Jira2Harvest.Dto
{
    public class HarvestTimeEntriesListDto
    {
        [JsonPropertyName("time_entries")]
        public HarvestTimeEntriesDto[] TimeEntries { get; set; } = [];
    }
}
