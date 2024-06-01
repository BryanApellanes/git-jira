using GitJira.Interfaces;
using Octokit;

namespace GitJira.Classes;

public class CompositeIssueResolver : ICompositeIssueResolver
{
    public CompositeIssueResolver(IGitHubClientProvider gitHubClientProvider, IJiraClientProvider jiraClientProvider)
    {
        this.GitHubClientProvider = gitHubClientProvider;
        this.JiraClientProvider = jiraClientProvider;
    }
    
    protected IGitHubClientProvider GitHubClientProvider { get; init; }
    protected IJiraClientProvider JiraClientProvider { get; init; }

    public async Task<ICompositeIssue> GetCompositeIssue(string githubOwner, string githubRepo, int githubIssueNumber)
    {
        GitHubClient gitHubClient = GitHubClientProvider.GetGitHubClient();
        return await GetCompositeIssue(await gitHubClient.Issue.Get(githubOwner, githubRepo, githubIssueNumber));
    }

    public Task<ICompositeIssue> GetCompositeIssue(Octokit.Issue issue)
    {
        throw new NotImplementedException();
    }

    public Task<ICompositeIssue> GetCompositeIssue(string jiraId)
    {
        throw new NotImplementedException();
    }

    public Task<ICompositeIssue> GetCompositeIssue(Atlassian.Jira.Issue issue)
    {
        throw new NotImplementedException();
    }

    public Task<bool> JiraIssueExistsAsync(GitHubIssueIdentifier gitHubIssueIdentifier)
    {
        throw new NotImplementedException();
    }

    public Task<bool> JiraIssueExistsAsync(GitHubIssueIdentifier gitHubIssueIdentifier, out Atlassian.Jira.Issue jiraIssue)
    {
        throw new NotImplementedException();
    }

    public Task<bool> JiraIssueExistsAsync(string githubOwner, string githubRepo, int githubIssueNumber)
    {
        throw new NotImplementedException();
    }

    public Task<bool> JiraIssueExistsAsync(string githubOwner, string githubRepo, int githubIssueNumber, out Atlassian.Jira.Issue jiraIssue)
    {
        throw new NotImplementedException();
    }

    public Task<bool> JiraIssueExistsAsync(Issue gitHubIssue)
    {
        throw new NotImplementedException();
    }

    public Task<bool> JiraIssueExistsAsync(Issue gitHubIssue, out Atlassian.Jira.Issue jiraIssue)
    {
        throw new NotImplementedException();
    }

    public Task<bool> GitIssueExistsAsync(string jiraId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> GitIssueExistsAsync(string jiraId, out Issue gitHubIssue)
    {
        throw new NotImplementedException();
    }

    public Task<bool> GitIssueExistsAsync(Atlassian.Jira.Issue jiraIssue)
    {
        throw new NotImplementedException();
    }

    public Task<bool> GitIssueExistsAsync(Atlassian.Jira.Issue jiraIssue, out Issue gitHubIssue)
    {
        throw new NotImplementedException();
    }
    
    protected Task<Issue> GetGitHubIssueAsync(string owner, string repo, int number)
    {
        GitHubClient gitHubClient = GitHubClientProvider.GetGitHubClient();
        return gitHubClient.Issue.Get(owner, repo, number);
    }
}