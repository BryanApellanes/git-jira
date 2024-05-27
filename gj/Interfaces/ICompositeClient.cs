using Atlassian.Jira;
using Octokit;

namespace GitJira.Interfaces;

public interface ICompositeClient
{
    Task<ICompositeClient> AddComment(IIssueCrossReference crossReference);
    Task<ICompositeClient> AddGitHubComment(IIssueCrossReference crossReference);
    Task<ICompositeClient> AddGitHubComment(string owner, string repo, int number);
    Task<ICompositeClient> AddGitHubComment(Octokit.Issue gitHubIssue);
    
    Task<ICompositeClient> AddJiraComment(IIssueCrossReference crossReference);
    Task<ICompositeClient> AddJiraComment(string jiraId);
    
    Task<List<Octokit.IssueComment>> GetGitHubCommentsAsync(string owner, string repo, int number);
    Task<List<Octokit.IssueComment>> GetGitHubCommentsAsync(Octokit.Issue gitHubIssue);
    Task<bool> JiraIssueExistsAsync(string githubOwner, string githubRepo, int githubIssueNumber);
    Task<bool> JiraIssueExistsAsync(Octokit.Issue gitHubIssue);

    Task<bool> GitIssueExistsAsync(string jiraId);
    Task<bool> GitIssueExistsAsync(Atlassian.Jira.Issue jiraIssue);

    Task<bool> HasReplyAsync(string owner, string repo, int number);
    /// <summary>
    /// Gets a value indicating if a comment has posted to the issue by someone other than the user who opened the issue.
    /// </summary>
    /// <param name="gitIssue"></param>
    /// <returns></returns>
    Task<bool> HasReplyAsync(Octokit.Issue gitIssue);

    Task<Octokit.Issue> GetGitHubIssueAsync(IIssueCrossReference crossReference);
    
    Task<Octokit.Issue> GetGitHubIssueAsync(string owner, string repo, int number);
    
    Task<Atlassian.Jira.Issue> GetJiraIssueAsync(IIssueCrossReference crossReference);
    
    Task<Atlassian.Jira.Issue> GetJiraIssueAsync(string jiraId);
}