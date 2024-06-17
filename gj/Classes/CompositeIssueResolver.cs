using GitJira.Interfaces;
using Octokit;

namespace GitJira.Classes;

public class CompositeIssueResolver : ICompositeIssueResolver
{
    public CompositeIssueResolver(IGitHubClientProvider gitHubClientProvider, IJiraClientProvider jiraClientProvider, IJiraReferenceResolver jiraReferenceResolver)
    {
        this.GitHubClientProvider = gitHubClientProvider;
        this.JiraClientProvider = jiraClientProvider;
        this.JiraReferenceResolver = jiraReferenceResolver;
    }
    
    protected IGitHubClientProvider GitHubClientProvider { get; init; }
    protected IJiraClientProvider JiraClientProvider { get; init; }
    
    protected IJiraReferenceResolver JiraReferenceResolver { get; init; }

    public async Task<ICompositeIssue> GetCompositeIssue(ICompositeClient compositeClient, GitHubIssueIdentifier gitHubIssueIdentifier)
    {
        GitHubClient gitHubClient = GitHubClientProvider.GetGitHubClient();
        Octokit.Issue githubIssue = await gitHubClient.Issue.Get(gitHubIssueIdentifier.Owner,
            gitHubIssueIdentifier.RepoName, gitHubIssueIdentifier.IssueNumber);

        await JiraIssueExistsAsync(gitHubIssueIdentifier, out Atlassian.Jira.Issue jiraIssue);

        return new CompositeIssue(githubIssue, jiraIssue)
        {
            CompositeClient = compositeClient,
            GitHubIssueIdentifier = gitHubIssueIdentifier
        };
    }

    public Task<bool> JiraIssueExistsAsync(GitHubIssueIdentifier gitHubIssueIdentifier, out Atlassian.Jira.Issue jiraIssue)
    {
        bool inComments = this.JiraReferenceResolver.JiraReferenceExistsInCommentsAsync(gitHubIssueIdentifier, out jiraIssue).Result;
        if (inComments)
        {
            return Task.FromResult(true);
        }

        bool asLabel = this.JiraReferenceResolver
            .JiraReferenceLabelExistsInCommentsAsync(gitHubIssueIdentifier, out jiraIssue).Result;

        return Task.FromResult(asLabel);
    }

    protected Task<Issue> GetGitHubIssueAsync(string owner, string repo, int number)
    {
        GitHubClient gitHubClient = GitHubClientProvider.GetGitHubClient();
        return gitHubClient.Issue.Get(owner, repo, number);
    }
}