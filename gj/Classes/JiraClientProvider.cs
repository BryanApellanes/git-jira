using Atlassian.Jira;
using Bam;
using GitJira.Interfaces;

namespace GitJira.Classes;

public class JiraClientProvider : IJiraClientProvider
{
    public JiraClientProvider(IJiraSettingsProvider jiraSettingsProvider)
    {
        this.JiraSettingsProvider = jiraSettingsProvider;
    }
    
    public IJiraSettingsProvider JiraSettingsProvider { get; init; }
    
    public Jira GetJiraClient()
    {
        IJiraSettings settings = this.JiraSettingsProvider.GetJiraSettings();
        return Jira.CreateRestClient(settings.Url, settings.Credentials.UserName, settings.Credentials.Password);
    }
}