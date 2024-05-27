namespace GitJira.Interfaces;

public interface IJiraClientProvider
{
    Atlassian.Jira.Jira GetJiraClient();
}