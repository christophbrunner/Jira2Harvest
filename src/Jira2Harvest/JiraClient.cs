using BSW.Core.Extension;
using Jira2Harvest.Dto;
using Microsoft.Extensions.Configuration;

namespace Jira2Harvest
{
    public class JiraClient : BaseClient
    {
        private readonly JiraUserDto _me;

        public JiraClient(IConfiguration configuration): base(configuration)
        {
            var jiraSettings = configuration.GetConfiguration<JiraSettings>("Jira");

            HttpClient.BaseAddress = new Uri(jiraSettings.JiraBaseUrl);

            HttpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {jiraSettings.Token}");

            _me = GetMe().Result;

            Console.WriteLine($"Jira user: {_me.Name}");

        }

        public async Task<JiraUserDto> GetMe()
        {
            return await Get<JiraUserDto>("/rest/api/2/myself");
        }

        public async Task<IssueListDto> GetIssuesWithWorkLog(DateTime date)
        {
            return await Get<IssueListDto>($"/rest/api/2/search?jql=worklogAuthor = {_me.Name} AND worklogDate = {date:yyyy-MM-dd}");
        }

        public async Task<JiraWorkLogListDto> GetWorkLog(string issue)
        {
            return await Get<JiraWorkLogListDto>($"/rest/api/2/issue/{issue}/worklog");
        }

        public async Task<decimal> GetWorkHours(DateTime date)
        {
            decimal hours = 0;
            var issues = await GetIssuesWithWorkLog(date);

            foreach (var issue in issues.Issues)
            {
                var workLogs = await GetWorkLog(issue.Key);

                foreach (var workLog in workLogs.WorkLogs.Where(x=>x.Started.Date == date.Date && x.Author.Name.Equals(_me.Name, StringComparison.InvariantCultureIgnoreCase)))
                {
                    hours += workLog.TimeSpentSeconds / 3600m;
                }
            }

            return hours;
        }

        public Task CheckSettings()
        {
            //currently no settings to check
            return Task.CompletedTask;
        }
    }
}