using Atlassian.Jira;
using GitJira.Interfaces;

namespace GitJira.Classes;

public class JiraClientProvider : IJiraClientProvider
{
    public JiraClientProvider(IJiraSettingsProvider jiraSettingsProvider)
    {
        this.JiraSettingsProvider = jiraSettingsProvider;
    }
    
    public IJiraSettingsProvider JiraSettingsProvider { get; init; }

    private Jira _jira;
    public Jira GetJiraClient()
    {
        if (_jira == null)
        {
            
            IJiraSettings settings = this.JiraSettingsProvider.GetJiraSettings();
            _jira = Jira.CreateRestClient(settings.Url, settings.Credentials.UserName, settings.Credentials.Password);
        }

        return _jira;
    }
}