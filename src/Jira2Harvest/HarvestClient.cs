using BSW.Core.Extension;
using Jira2Harvest.Dto;
using Microsoft.Extensions.Configuration;

namespace Jira2Harvest;

public class HarvestClient : BaseClient
{
    private readonly HarvestSettings _harvestSettings;
    private readonly HarvestFullUserDto _me;

    private long _harvestProjectId;

    private readonly ConsoleService _consoleService;

    public HarvestClient(IConfiguration configuration, ConsoleService consoleService): base(configuration)
    {
        _consoleService = consoleService;
        _harvestSettings = configuration.GetConfiguration<HarvestSettings>("Harvest");

        HttpClient.BaseAddress = new Uri("https://api.harvestapp.com/api/v2/");

        HttpClient.DefaultRequestHeaders.Add("Harvest-Account-ID", _harvestSettings.HarvestAccountId.ToString());
        HttpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_harvestSettings.AccessToken}");
        HttpClient.DefaultRequestHeaders.Add("User-Agent", $"Jira to Harvest");

        _me = GetMe().Result;

        Console.WriteLine($"Harvest user: {_me.Email}");
    }

    public async Task CheckSettings()
    {
        var allMyProjects = await GetMyProjects();

        if (_harvestSettings.TaskId <= 0)
        {
            foreach (var projectAssignment in allMyProjects.ProjectAssignments)
            {
                Console.WriteLine($"> {projectAssignment.Client.Name} - {projectAssignment.Project.Name}");

                foreach (var taskAssignment in projectAssignment.TaskAssignments)
                {
                    Console.WriteLine($"  > {taskAssignment.Task.Name} ({taskAssignment.Task.Id})");
                }
            }

            const string message = "Harvest configuration Error. TaskId is missing. Add valid TaskId";

            _consoleService.WriteLine(message, ConsoleColor.Red);

            throw new Exception(message);
        }

        _harvestProjectId = allMyProjects.ProjectAssignments
            .FirstOrDefault(x => x.TaskAssignments.Any(assignments => assignments.Task.Id == _harvestSettings.TaskId))?.Project.Id ?? 0;

        if (_harvestProjectId <= 0)
        {
            string message = "Harvest configuration Error. TaskId is invalid. Add valid TaskId";

            _consoleService.WriteLine(message, ConsoleColor.Red);

            throw new Exception(message);
        }
    }

    public async Task<HarvestTaskDto> GetTask(long taskId)
    {
        return await Get<HarvestTaskDto>($"tasks/{taskId}");
    }

    public async Task<HarvestFullUserDto> GetMe()
    {
        return await Get<HarvestFullUserDto>("users/me");
    }

    public async Task<HarvestProjectAssignmentsListDto> GetMyProjects()
    {
        return await Get<HarvestProjectAssignmentsListDto>("users/me/project_assignments");
    }

    public async Task<HarvestTimeEntriesDto?> GetRelevantTimeEntry(DateTime date)
    {
        var allTimeEntries = await GetTimeEntries(date);

        return allTimeEntries.TimeEntries.FirstOrDefault(x => x.Task.Id == _harvestSettings.TaskId &&
                                                              x.User.Id == _me.Id);
    }

    public async Task<HarvestTimeEntriesListDto> GetTimeEntries(DateTime date)
    {
        return await Get<HarvestTimeEntriesListDto>($"time_entries?from={date:yyyy-MM-dd}&to={date:yyyy-MM-dd}");
    }

    public async Task<HarvestTimeEntriesDto> UpdateTimeEntry(long? foundTimeTrackId, DateTime start, decimal hours)
    {
        return await Patch<HarvestTimeEntriesDto>($"time_entries/{foundTimeTrackId}?spent_date={start:yyyy-MM-dd}&hours={hours}");
    }

    public async Task<HarvestTimeEntriesDto> DeleteTimeEntry(long? foundTimeTrackId)
    {
        return await Delete<HarvestTimeEntriesDto>($"time_entries/{foundTimeTrackId}");
    }

    public async Task<HarvestTimeEntriesDto> AddTimeEntry(DateTime start, decimal hours)
    {
        return await Post<HarvestTimeEntriesDto>($"time_entries?project_id={_harvestProjectId}&task_id={_harvestSettings.TaskId}&spent_date={start:yyyy-MM-dd}&hours={hours}");
    }
}