using Atlassian.Jira;
using GitJira.Classes;
using Octokit;

namespace GitJira.Interfaces;

public interface ICompositeClient
{
    Task<List<Octokit.Repository>> ListRepositoriesForUser(bool refresh = false);
    Task<List<Octokit.Repository>> ListRepositoriesForUser(string userName, bool refresh = false);
    Task<List<Octokit.Repository>> ListRepositoriesForOrg(bool refresh = false);
    Task<List<Octokit.Repository>> ListRepositoriesForOrg(string orgName, bool refresh = false);

    Task<Repository> GetRepository(GitHubRepoIdentifier gitHubRepoIdentifier);
    Task<Octokit.Repository> GetRepository(string owner, string repoName);
    Task<List<Octokit.Issue>> ListGitHubIssuesAsync();
    Task<List<Octokit.Issue>> ListGitHubIssuesAsync(GitHubRepoIdentifier gitHubRepoIdentifier);
    Task<List<Octokit.Issue>> ListGitHubIssuesAsync(string owner, string repoName);
    Task<ICompositeClient> AddCommentAsync(ICompositeIssue crossReference);
    Task<ICompositeClient> AddGitHubCommentAsync(ICompositeIssue crossReference);
    Task<ICompositeClient> AddGitHubCommentAsync(string owner, string repo, int number);
    Task<ICompositeClient> AddGitHubCommentAsync(Octokit.Issue gitHubIssue);
    
    Task<ICompositeClient> AddJiraCommentAsync(ICompositeIssue crossReference);
    Task<ICompositeClient> AddJiraCommentAsync(string jiraId);
    
    Task<List<Octokit.IssueComment>> GetGitHubCommentsAsync(GitHubIssueIdentifier gitHubIssueIdentifier);
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
    Task<bool> HasReplyAsync(Octokit.Issue gitIssue, out Octokit.IssueComment? comment);

    Task<Octokit.Issue> GetGitHubIssueAsync(ICompositeIssue crossReference);
    Task<Octokit.Issue> GetGitHubIssueAsync(GitHubIssueIdentifier gitHubIssueIdentifier);
    Task<Octokit.Issue> GetGitHubIssueAsync(string owner, string repo, int number);
    
    Task<Atlassian.Jira.Issue> GetJiraIssueAsync(ICompositeIssue crossReference);
    
    Task<Atlassian.Jira.Issue> GetJiraIssueAsync(string jiraId);
}