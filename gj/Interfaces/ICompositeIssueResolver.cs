using GitJira.Classes;

namespace GitJira.Interfaces;

public interface ICompositeIssueResolver
{
    Task<ICompositeIssue> GetCompositeIssue(ICompositeClient compositeClient, GitHubIssueIdentifier gitHubIssueIdentifier);
    
    Task<bool> JiraIssueExistsAsync(GitHubIssueIdentifier gitHubIssueIdentifier, out Atlassian.Jira.Issue jiraIssue);
}
