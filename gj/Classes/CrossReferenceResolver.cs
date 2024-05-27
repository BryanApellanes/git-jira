using GitJira.Interfaces;
using Octokit;

namespace GitJira.Classes;

public class CrossReferenceResolver : ICrossReferenceResolver
{
    public CrossReferenceResolver(IGitHubClientProvider gitHubClientProvider, IJiraClientProvider jiraClientProvider)
    {
        this.GitHubClientProvider = gitHubClientProvider;
        this.JiraClientProvider = jiraClientProvider;
    }
    
    protected IGitHubClientProvider GitHubClientProvider { get; init; }
    protected IJiraClientProvider JiraClientProvider { get; init; }

    public IIssueCrossReference GetCrossReference(string githubOwner, string githubRepo, int githubIssueNumber)
    {
        throw new NotImplementedException();
    }

    public IIssueCrossReference GetCrossReference(Octokit.Issue issue)
    {
        throw new NotImplementedException();
    }

    public IIssueCrossReference GetCrossReference(string jiraId)
    {
        throw new NotImplementedException();
    }

    public IIssueCrossReference GetCrossReference(Atlassian.Jira.Issue issue)
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