using Atlassian.Jira;
using Bam;
using GitJira.Interfaces;
using Octokit;
using Issue = Octokit.Issue;

namespace GitJira.Classes;

public class CompositeClient : ICompositeClient
{
    public CompositeClient(IJiraClientProvider jiraClientProvider, IGitHubClientProvider gitHubClientProvider, ICrossReferenceResolver crossReferenceResolver, ISettingsProvider settingsProvider)
    {
        this.JiraClientProvider = jiraClientProvider;
        this.GitHubClientProvider = gitHubClientProvider;
        this.SettingsProvider = settingsProvider;
    }
    
    public IJiraClientProvider JiraClientProvider { get; init; }
    public IGitHubClientProvider GitHubClientProvider { get; init; }
    
    public ICrossReferenceResolver CrossReferenceResolver { get; init; }
    
    public ISettingsProvider SettingsProvider { get; init; }

    public Task<ICompositeClient> AddComment(IIssueCrossReference crossReference)
    {
        throw new NotImplementedException();
    }

    public Task<ICompositeClient> AddGitHubComment(IIssueCrossReference crossReference)
    {
        throw new NotImplementedException();
    }

    public Task<ICompositeClient> AddGitHubComment(string owner, string repo, int number)
    {
        throw new NotImplementedException();
    }

    public Task<ICompositeClient> AddGitHubComment(Octokit.Issue gitHubIssue)
    {
        throw new NotImplementedException();
    }

    public Task<ICompositeClient> AddJiraComment(IIssueCrossReference crossReference)
    {
        throw new NotImplementedException();
    }

    public Task<ICompositeClient> AddJiraComment(string jiraId)
    {
        throw new NotImplementedException();
    }

    public async Task<List<IssueComment>> GetGitHubCommentsAsync(string owner, string repo, int number)
    {
        return await GetGitHubCommentsAsync(await GetGitHubIssueAsync(owner, repo, number));
    }

    public async Task<List<IssueComment>> GetGitHubCommentsAsync(Octokit.Issue gitHubIssue)
    {
        HttpClient httpClient = new HttpClient();
        HttpResponseMessage response = await httpClient.GetAsync(gitHubIssue.CommentsUrl);
        response.EnsureSuccessStatusCode();
        string json = await response.Content.ReadAsStringAsync();
        if (!string.IsNullOrEmpty(json))
        {
            return json.FromJson<List<IssueComment>>();
        }

        return new List<IssueComment>();
    }

    public async Task<bool> JiraIssueExistsAsync(string githubOwner, string githubRepo, int githubIssueNumber)
    {
        return await JiraIssueExistsAsync(await GetGitHubIssueAsync(githubOwner, githubRepo, githubIssueNumber));
    }

    public Task<bool> JiraIssueExistsAsync(Issue gitHubIssue)
    {
        return CrossReferenceResolver.JiraIssueExistsAsync(gitHubIssue);
    }

    public Task<bool> GitIssueExistsAsync(string jiraId)
    {
        return CrossReferenceResolver.GitIssueExistsAsync(jiraId);
    }

    public Task<bool> GitIssueExistsAsync(Atlassian.Jira.Issue jiraIssue)
    {
        return CrossReferenceResolver.GitIssueExistsAsync(jiraIssue);
    }

    public async Task<bool> HasReplyAsync(string owner, string repo, int number)
    {
        return await HasReplyAsync(await GetGitHubIssueAsync(owner, repo, number));
    }

    public async Task<bool> HasReplyAsync(Issue gitIssue)
    {
        HashSet<string> repliers = SettingsProvider.GetSettings().Repliers;
        List<IssueComment> comments = (await GetGitHubCommentsAsync(gitIssue)).ToList();
        if (comments.Any())
        {
            foreach (IssueComment comment in comments)
            {
                if (repliers.Contains(comment.User.Email))
                {
                    return true;
                }
            }
        }

        return false;
    }

    public Task<Issue> GetGitHubIssueAsync(IIssueCrossReference crossReference)
    {
        return GetGitHubIssueAsync(crossReference.GitHubOwner, crossReference.GitHubRepo, crossReference.GitHubIssueNumber);
    }

    public Task<Issue> GetGitHubIssueAsync(string owner, string repo, int number)
    {
        GitHubClient gitHubClient = GitHubClientProvider.GetGitHubClient();
        return gitHubClient.Issue.Get(owner, repo, number);
    }

    public Task<Atlassian.Jira.Issue> GetJiraIssueAsync(IIssueCrossReference crossReference)
    {
        return GetJiraIssueAsync(crossReference.JiraId);
    }

    public async Task<Atlassian.Jira.Issue> GetJiraIssueAsync(string jiraId)
    {
        Jira jira = JiraClientProvider.GetJiraClient();
        return await jira.Issues.GetIssueAsync(jiraId);
    }
}