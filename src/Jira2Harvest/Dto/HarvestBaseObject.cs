namespace Jira2Harvest.Dto;

public abstract class HarvestBaseObject
{
    public required long Id { get; set; }
    public required string Name { get; set; }
}