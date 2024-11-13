using System.Text.Json.Serialization;

namespace Jira2Harvest.Dto
{
    public class HarvestProjectAssignmentsDto
    {
        public required HarvestProjectDto Project { get; set; }
        public required HarvestClientDto Client { get; set; }
        [JsonPropertyName("task_assignments")]
        public required HarvestTaskAssignmentsDto[] TaskAssignments { get; set; }
    }

    public class HarvestTaskAssignmentsDto
    {
        public required HarvestTaskDto Task { get; set; }
    }
}
