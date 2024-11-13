using System.Text.Json.Serialization;

namespace Jira2Harvest.Dto
{
    public class HarvestTimeEntriesDto
    {
        public long Id { get; set; }

        public required HarvestProjectDto Project { get; set; }

        public required HarvestTaskDto Task { get; set; }

        public required HarvestUserDto User { get; set; }

        [JsonPropertyName("rounded_hours")]
        public required decimal RoundedHours { get; set; }

        [JsonPropertyName("spent_date")]
        public required DateTime SpentDate { get; set; }

        [JsonPropertyName("is_locked")]
        public required bool IsLocked { get; set; }
    }
}
