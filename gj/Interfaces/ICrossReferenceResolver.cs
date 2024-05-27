namespace GitJira.Interfaces;

public interface ICrossReferenceResolver
{
    IIssueCrossReference GetCrossReference(string githubOwner, string githubRepo, int githubIssueNumber);
    IIssueCrossReference GetCrossReference(Octokit.Issue issue);

    IIssueCrossReference GetCrossReference(string jiraId);
    IIssueCrossReference GetCrossReference(Atlassian.Jira.Issue issue);

    Task<bool> JiraIssueExistsAsync(string githubOwner, string githubRepo, int githubIssueNumber);
    Task<bool> JiraIssueExistsAsync(string githubOwner, string githubRepo, int githubIssueNumber, out Atlassian.Jira.Issue jiraIssue);
    Task<bool> JiraIssueExistsAsync(Octokit.Issue gitHubIssue);

    Task<bool> JiraIssueExistsAsync(Octokit.Issue gitHubIssue, out Atlassian.Jira.Issue jiraIssue);
    
    Task<bool> GitIssueExistsAsync(string jiraId);
    Task<bool> GitIssueExistsAsync(string jiraId, out Octokit.Issue gitHubIssue);
    Task<bool> GitIssueExistsAsync(Atlassian.Jira.Issue jiraIssue);
    Task<bool> GitIssueExistsAsync(Atlassian.Jira.Issue jiraIssue, out Octokit.Issue gitHubIssue);
}
