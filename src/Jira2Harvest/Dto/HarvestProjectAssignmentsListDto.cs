using System.Text.Json.Serialization;

namespace Jira2Harvest.Dto;

public class HarvestProjectAssignmentsListDto
{
    [JsonPropertyName("project_assignments")]
    public HarvestProjectAssignmentsDto[] ProjectAssignments { get; set; } = [];
}