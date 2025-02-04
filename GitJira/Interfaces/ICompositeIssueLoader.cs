using GitJira.Classes;

namespace GitJira.Interfaces;

public interface ICompositeIssueLoader
{
    Task<ICompositeIssue> LoadCompositeIssueAsync(ICompositeIssueManager compositeIssueManager, GitHubIssueIdentifier gitHubIssueIdentifier);
    
    Task<bool> JiraIssueExistsAsync(GitHubIssueIdentifier gitHubIssueIdentifier, out Atlassian.Jira.Issue jiraIssue);
}
