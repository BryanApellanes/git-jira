using Atlassian.Jira;
using GitJira.Classes;
using Octokit;

namespace GitJira.Interfaces;

public interface ICompositeClient
{
    void DeleteMe();
    IEnumerable<ICompositeIssue> GetCompositeIssues(GitHubRepoIdentifier gitHubRepoIdentifier);
    Task<List<ICompositeIssue>> GetCompositeIssuesAsync(GitHubRepoIdentifier gitHubRepoIdentifier);
    Task<List<Octokit.Issue>> ListGitHubIssuesAsync(GitHubRepoIdentifier gitHubRepoIdentifier);

    Task<List<Octokit.Issue>> ListGitHubIssuesAsync(string owner, string repoName);
    
    Task<List<Octokit.IssueComment>> GetGitHubCommentsAsync(GitHubIssueIdentifier gitHubIssueIdentifier);
    Task<List<Octokit.IssueComment>> GetGitHubCommentsAsync(string owner, string repo, int issueNumber);
    
    Task<List<Octokit.IssueComment>> GetGitHubCommentsAsync(Octokit.Issue gitHubIssue);
    Task<bool> JiraIssueExistsAsync(GitHubIssueIdentifier gitHubIssueIdentifier, out Atlassian.Jira.Issue jiraIssue);

    /// <summary>
    /// Gets a value indicating if a comment has posted to the issue by someone other than the user who opened the issue.
    /// </summary>
    /// <param name="gitIssue"></param>
    /// <returns></returns>
    Task<bool> HasReplyAsync(Octokit.Issue gitIssue);
    Task<bool> HasReplyAsync(Octokit.Issue gitIssue, out Octokit.IssueComment? comment);
    Task<Octokit.Issue> GetGitHubIssueAsync(GitHubIssueIdentifier gitHubIssueIdentifier);
    Task<Octokit.Issue> GetGitHubIssueAsync(string owner, string repo, int number);

    Task<Atlassian.Jira.Issue> CreateJiraIssueAsync(ICompositeIssue compositeIssue);
    Task<Octokit.IssueComment> AddGithubComment(ICompositeIssue compositeIssue, string comment);
    Task<Atlassian.Jira.Issue> GetJiraIssueAsync(string jiraId);
}