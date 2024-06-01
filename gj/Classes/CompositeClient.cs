using Atlassian.Jira;
using Bam;
using GitJira.Interfaces;
using Octokit;
using Issue = Octokit.Issue;

namespace GitJira.Classes;

public class CompositeClient : ICompositeClient
{
    public CompositeClient(IJiraClientProvider jiraClientProvider, IGitHubClientProvider gitHubClientProvider, ICompositeIssueResolver compositeIssueResolver, ISettingsProvider settingsProvider)
    {
        this.JiraClientProvider = jiraClientProvider;
        this.GitHubClientProvider = gitHubClientProvider;
        this.SettingsProvider = settingsProvider;
    }
    
    public IJiraClientProvider JiraClientProvider { get; init; }
    public IGitHubClientProvider GitHubClientProvider { get; init; }
    
    public ICompositeIssueResolver CompositeIssueResolver { get; init; }
    
    public ISettingsProvider SettingsProvider { get; init; }

    public async Task<List<Repository>> ListRepositoriesForUser(bool refresh = false)
    {
        Settings settings = SettingsProvider.GetSettings();
        return await ListRepositoriesForUser(settings.GitSettings.UserName);
    }

    public async Task<List<Repository>> ListRepositoriesForUser(string userName, bool refresh = false)
    {
        GitHubClient client = GitHubClientProvider.GetGitHubClient();
        return (await client.Repository.GetAllForUser(userName)).ToList();
    }

    public async Task<List<Repository>> ListRepositoriesForOrg(bool refresh = false)
    {
        Settings settings = SettingsProvider.GetSettings();
        return await ListRepositoriesForOrg(settings.GitSettings.OrgName);
    }

    public async Task<List<Repository>> ListRepositoriesForOrg(string orgName, bool refresh = false)
    {
        GitHubClient client = GitHubClientProvider.GetGitHubClient();
        return (await client.Repository.GetAllForOrg(orgName)).ToList();
    }

    public Task<Repository> GetRepository(GitHubRepoIdentifier gitHubRepoIdentifier)
    {
        return GetRepository(gitHubRepoIdentifier.Owner, gitHubRepoIdentifier.RepositoryName);
    }

    public Task<Repository> GetRepository(string owner, string repoName)
    {
        GitHubClient client = GitHubClientProvider.GetGitHubClient();
        return client.Repository.Get(owner, repoName);
    }

    public async Task<List<Issue>> ListGitHubIssuesAsync()
    {
        Settings settings = SettingsProvider.GetSettings();
        List<Issue> issues = new List<Issue>();
        if (!string.IsNullOrEmpty(settings.GitSettings.OrgName))
        {
            issues.AddRange(await ListGitHubIssuesAsync(settings.GitSettings.GetOrgRepoIdentifier()));
        }

        if (!string.IsNullOrEmpty(settings.GitSettings.UserName))
        {
            issues.AddRange(await ListGitHubIssuesAsync(settings.GitSettings.GetUserRepoIdentifier()));
        }

        return issues;
    }

    public async Task<List<Issue>> ListGitHubIssuesAsync(GitHubRepoIdentifier gitHubRepoIdentifier)
    {
        return await ListGitHubIssuesAsync(gitHubRepoIdentifier.Owner, gitHubRepoIdentifier.RepositoryName);
    }

    public async Task<List<Issue>> ListGitHubIssuesAsync(string owner, string repoName)
    {
        GitHubClient client = GitHubClientProvider.GetGitHubClient();
        return (await client.Issue.GetAllForRepository(owner, repoName)).ToList();
    }

    public Task<ICompositeClient> AddCommentAsync(ICompositeIssue crossReference)
    {
        throw new NotImplementedException();
    }

    public Task<ICompositeClient> AddGitHubCommentAsync(ICompositeIssue crossReference)
    {
        throw new NotImplementedException();
    }

    public Task<ICompositeClient> AddGitHubCommentAsync(string owner, string repo, int number)
    {
        throw new NotImplementedException();
    }

    public Task<ICompositeClient> AddGitHubCommentAsync(Octokit.Issue gitHubIssue)
    {
        throw new NotImplementedException();
    }

    public Task<ICompositeClient> AddJiraCommentAsync(ICompositeIssue crossReference)
    {
        throw new NotImplementedException();
    }

    public Task<ICompositeClient> AddJiraCommentAsync(string jiraId)
    {
        throw new NotImplementedException();
    }

    public Task<List<IssueComment>> GetGitHubCommentsAsync(GitHubIssueIdentifier gitHubIssueIdentifier)
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
        return CompositeIssueResolver.JiraIssueExistsAsync(gitHubIssue);
    }

    public Task<bool> GitIssueExistsAsync(string jiraId)
    {
        return CompositeIssueResolver.GitIssueExistsAsync(jiraId);
    }

    public Task<bool> GitIssueExistsAsync(Atlassian.Jira.Issue jiraIssue)
    {
        return CompositeIssueResolver.GitIssueExistsAsync(jiraIssue);
    }

    public async Task<bool> HasReplyAsync(string owner, string repo, int number)
    {
        return await HasReplyAsync(await GetGitHubIssueAsync(owner, repo, number));
    }

    public async Task<bool> HasReplyAsync(Issue gitIssue)
    {
        return await HasReplyAsync(gitIssue, out _);
    }

    public Task<bool> HasReplyAsync(Issue gitIssue, out IssueComment? comment)
    {
        HashSet<string> repliers = SettingsProvider.GetSettings().Repliers;
        List<IssueComment> comments = GetGitHubCommentsAsync(gitIssue).Result.ToList();
        if (comments.Any())
        {
            foreach (IssueComment c in comments)
            {
                if (repliers.Contains(c.User.Email))
                {
                    comment = c;
                    return Task.FromResult(true);
                }
            }
        }

        comment = null;
        return Task.FromResult(false);
    }

    public Task<Issue> GetGitHubIssueAsync(ICompositeIssue crossReference)
    {
        return GetGitHubIssueAsync(crossReference.GitHubIssueId);
    }

    public Task<Issue> GetGitHubIssueAsync(GitHubIssueIdentifier gitHubIssueIdentifier)
    {
        if (gitHubIssueIdentifier == null)
        {
            throw new ArgumentNullException(nameof(gitHubIssueIdentifier));
        }
        
        return GetGitHubIssueAsync(gitHubIssueIdentifier.Owner, gitHubIssueIdentifier.RepoName,
            gitHubIssueIdentifier.IssueNumber);
    }

    public Task<Issue> GetGitHubIssueAsync(string owner, string repo, int number)
    {
        GitHubClient gitHubClient = GitHubClientProvider.GetGitHubClient();
        return gitHubClient.Issue.Get(owner, repo, number);
    }

    public Task<Atlassian.Jira.Issue> GetJiraIssueAsync(ICompositeIssue crossReference)
    {
        return GetJiraIssueAsync(crossReference.JiraId);
    }

    public async Task<Atlassian.Jira.Issue> GetJiraIssueAsync(string jiraId)
    {
        Jira jira = JiraClientProvider.GetJiraClient();
        return await jira.Issues.GetIssueAsync(jiraId);
    }
}