using System.Text.Json.Serialization;
namespace Jira2Harvest.Dto;

public class JiraWorkLogDto
{
    public required int TimeSpentSeconds { get; set; }

    [JsonConverter(typeof(CustomDateTimeConverter))]
    public required DateTime Started { get; set; }

    public required JiraUserDto Author { get; set; }
}