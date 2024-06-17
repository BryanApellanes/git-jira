using GitJira.Classes;

namespace GitJira.Interfaces;

public interface IJiraReferenceResolver
{
    Task<bool> JiraReferenceExistsInCommentsAsync(GitHubIssueIdentifier gitHubIssueIdentifier, out Atlassian.Jira.Issue jiraIssue);
    Task<bool> JiraReferenceLabelExistsInCommentsAsync(GitHubIssueIdentifier gitHubIssueIdentifier, out Atlassian.Jira.Issue jiraIssue);

    bool ContainsJiraId(string body, out string jiraId);
}