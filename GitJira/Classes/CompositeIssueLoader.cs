using Atlassian.Jira;
using GitJira.Interfaces;
using Octokit;
using Issue = Octokit.Issue;

namespace GitJira.Classes;

public class CompositeIssueLoader : ICompositeIssueLoader
{
    public CompositeIssueLoader(IGitHubClientProvider gitHubClientProvider, IJiraClientProvider jiraClientProvider, IJiraReferenceResolver jiraReferenceResolver)
    {
        this.GitHubClientProvider = gitHubClientProvider;
        this.JiraClientProvider = jiraClientProvider;
        this.JiraReferenceResolver = jiraReferenceResolver;
    }
    
    protected IGitHubClientProvider GitHubClientProvider { get; init; }
    protected IJiraClientProvider JiraClientProvider { get; init; }
    
    protected IJiraReferenceResolver JiraReferenceResolver { get; init; }

    public async Task<ICompositeIssue> LoadCompositeIssueAsync(ICompositeIssueManager compositeIssueManager, GitHubIssueIdentifier gitHubIssueIdentifier)
    {
        GitHubClient gitHubClient = GitHubClientProvider.GetGitHubClient();
        Octokit.Issue githubIssue = await gitHubClient.Issue.Get(gitHubIssueIdentifier.Owner,
            gitHubIssueIdentifier.RepoName, gitHubIssueIdentifier.IssueNumber);

        await JiraIssueExistsAsync(gitHubIssueIdentifier, out Atlassian.Jira.Issue jiraIssue);

        return new CompositeIssue(githubIssue, jiraIssue)
        {
            CompositeIssueManager = compositeIssueManager,
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
            .JiraReferenceLabelExistsAsync(gitHubIssueIdentifier, out jiraIssue).Result;

        return Task.FromResult(asLabel);
    }
}