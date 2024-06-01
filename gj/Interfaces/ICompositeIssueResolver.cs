using GitJira.Classes;

namespace GitJira.Interfaces;

public interface ICompositeIssueResolver
{
    Task<ICompositeIssue> GetCompositeIssue(string githubOwner, string githubRepo, int githubIssueNumber);
    Task<ICompositeIssue> GetCompositeIssue(Octokit.Issue issue);

    Task<ICompositeIssue> GetCompositeIssue(string jiraId);
    Task<ICompositeIssue> GetCompositeIssue(Atlassian.Jira.Issue issue);

    Task<bool> JiraIssueExistsAsync(GitHubIssueIdentifier gitHubIssueIdentifier);
    Task<bool> JiraIssueExistsAsync(GitHubIssueIdentifier gitHubIssueIdentifier, out Atlassian.Jira.Issue jiraIssue);
    Task<bool> JiraIssueExistsAsync(string githubOwner, string githubRepo, int githubIssueNumber);
    Task<bool> JiraIssueExistsAsync(string githubOwner, string githubRepo, int githubIssueNumber, out Atlassian.Jira.Issue jiraIssue);
    Task<bool> JiraIssueExistsAsync(Octokit.Issue gitHubIssue);

    Task<bool> JiraIssueExistsAsync(Octokit.Issue gitHubIssue, out Atlassian.Jira.Issue jiraIssue);
    
    Task<bool> GitIssueExistsAsync(string jiraId);
    Task<bool> GitIssueExistsAsync(string jiraId, out Octokit.Issue gitHubIssue);
    Task<bool> GitIssueExistsAsync(Atlassian.Jira.Issue jiraIssue);
    Task<bool> GitIssueExistsAsync(Atlassian.Jira.Issue jiraIssue, out Octokit.Issue gitHubIssue);
}
