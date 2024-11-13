using Jira2Harvest.Dto;

namespace Jira2Harvest
{
    public class SyncService
    {
        private readonly HarvestClient _harvestClient;
        private readonly JiraClient _jiraClient;

        private readonly ConsoleService _consoleService;

        public SyncService(HarvestClient harvestClient, JiraClient jiraClient, ConsoleService consoleService)
        {
            _harvestClient = harvestClient;
            _jiraClient = jiraClient;
            _consoleService = consoleService;
        }

        public async Task CheckSettings()
        {
            await _jiraClient.CheckSettings();
            await _harvestClient.CheckSettings();
        }

        public async Task SyncAll()
        {
            DateTime start = DateTime.Now;

            while (true)
            {
                bool success = await SyncDay(start);

                if(!success)
                {
                    Console.WriteLine($"Locked entry found, stopping at {start:D}");
                    break;
                }

                start = start.AddDays(-1);
            }
        }

        public async Task<bool> SyncDay(DateTime date)
        {
            var workHours = await _jiraClient.GetWorkHours(date);

            var timeEntry = await _harvestClient.GetRelevantTimeEntry(date);

            if (timeEntry != null)
            {
                if (timeEntry.IsLocked)
                {
                    return false;
                }

                if (workHours == timeEntry.RoundedHours)
                {
                    //already in harvest, nothing to do
                    _consoleService.WriteLine(
                        $"Exist:\t\t{timeEntry.SpentDate:D} {timeEntry.RoundedHours}h ({timeEntry.Project.Name} -> {timeEntry.Task.Name})",
                        ConsoleColor.DarkGray);
                    return true;
                }

                if (workHours > 0)
                {
                    await UpdateTimeEntry(timeEntry, workHours);
                }
                else
                {
                    await DeleteTimeEntry(timeEntry);
                }
            }
            else
            {
                if (workHours > 0)
                {
                    await AddTimeEntry(date, workHours);
                }
                else
                {
                    //nothing in harvest and no work hours, nothing to do
                    _consoleService.WriteLine(
                        $"No work:\t{date:D}",
                        ConsoleColor.DarkYellow);
                    return true;
                }
            }

            return true;
        }

        private async Task AddTimeEntry(DateTime date, decimal workHours)
        {
            var entry = await _harvestClient.AddTimeEntry(date, workHours);

            _consoleService.WriteLine(
                $"Added:\t\t{entry.SpentDate:D} {entry.RoundedHours}h\t{entry.Project.Name} -> {entry.Task.Name}",
                ConsoleColor.Green);
        }

        private async Task UpdateTimeEntry(HarvestTimeEntriesDto timeEntry, decimal workHours)
        {
            var updated = await _harvestClient.UpdateTimeEntry(timeEntry.Id, timeEntry.SpentDate, workHours);

            _consoleService.WriteLine(
                $"Updated:\t{updated.SpentDate:D} from {timeEntry.RoundedHours} to {updated.RoundedHours}h ({updated.Project.Name} -> {updated.Task.Name})",
                ConsoleColor.DarkGreen);
        }

        private async Task DeleteTimeEntry(HarvestTimeEntriesDto timeEntry)
        {
            var updated = await _harvestClient.DeleteTimeEntry(timeEntry.Id);

            _consoleService.WriteLine(
                $"Deleted:\t{updated.SpentDate:D} from {timeEntry.RoundedHours} to {updated.RoundedHours}h ({updated.Project.Name} -> {updated.Task.Name})",
                ConsoleColor.Red);
        }
    }
}