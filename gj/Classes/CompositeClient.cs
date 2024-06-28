using System.Net.Http.Headers;
using Atlassian.Jira;
using Bam;
using Bam.Console;
using System.Linq;
using GitJira.Interfaces;
using MongoDB.Driver;
using Octokit;
using Issue = Octokit.Issue;

namespace GitJira.Classes;

public class CompositeClient : ICompositeClient
{
    public const string ProjectKey = "OKTA";
    public const string DefaultIssueType = "Internal Story";
    public const string DefaultComponent = "Team: Developer Community Products";
    
    public CompositeClient(IJiraClientProvider jiraClientProvider, IGitHubClientProvider gitHubClientProvider, ICompositeIssueResolver compositeIssueResolver, ISettingsProvider settingsProvider)
    {
        this.JiraClientProvider = jiraClientProvider;
        this.GitHubClientProvider = gitHubClientProvider;
        this.SettingsProvider = settingsProvider;
        this.CompositeIssueResolver = compositeIssueResolver;
    }
    
    public IJiraClientProvider JiraClientProvider { get; init; }
    public IGitHubClientProvider GitHubClientProvider { get; init; }
    
    public ICompositeIssueResolver CompositeIssueResolver { get; init; }
    
    public ISettingsProvider SettingsProvider { get; init; }

    public IEnumerable<ICompositeIssue> GetCompositeIssues(GitHubRepoIdentifier gitHubRepoIdentifier)
    {
        List<Issue> githubIssues = ListGitHubIssuesAsync(gitHubRepoIdentifier).Result;
        foreach (Issue issue in githubIssues)
        {
            if (issue.PullRequest != null) // filter out pull requests
            {
                continue;
            }
            yield return this.CompositeIssueResolver.GetCompositeIssue(this, new GitHubIssueIdentifier()
            {
                Owner = gitHubRepoIdentifier.Owner,
                RepoName = gitHubRepoIdentifier.RepositoryName,
                IssueNumber = issue.Number
            }).Result;
        }
    }
    public async Task<List<ICompositeIssue>> GetCompositeIssuesAsync(GitHubRepoIdentifier gitHubRepoIdentifier)
    {
        List<Issue> githubIssues = await ListGitHubIssuesAsync(gitHubRepoIdentifier);
        List<ICompositeIssue> results = new List<ICompositeIssue>();
        foreach (Issue issue in githubIssues)
        {
            results.Add(await this.CompositeIssueResolver.GetCompositeIssue(this, new GitHubIssueIdentifier()
            {
                Owner = gitHubRepoIdentifier.Owner,
                RepoName = gitHubRepoIdentifier.RepositoryName,
                IssueNumber = issue.Number
            }));
        }

        return results;
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

    public async Task<List<IssueComment>> GetGitHubCommentsAsync(GitHubIssueIdentifier gitHubIssueIdentifier)
    {
        GitHubClient client = GitHubClientProvider.GetGitHubClient();
        return
        [
            ..await client.Issue.Comment.GetAllForIssue(gitHubIssueIdentifier.Owner,
                gitHubIssueIdentifier.RepoName,
                gitHubIssueIdentifier.IssueNumber)
        ];
    }

    public async Task<List<IssueComment>> GetGitHubCommentsAsync(string owner, string repo, int number)
    {
        return await GetGitHubCommentsAsync(await GetGitHubIssueAsync(owner, repo, number));
    }

    public async Task<List<IssueComment>> GetGitHubCommentsAsync(Octokit.Issue gitHubIssue)
    {
        Settings settings = SettingsProvider.GetSettings();
        
        GitHubClient client = GitHubClientProvider.GetGitHubClient();
        return
        [
            ..await client.Issue.Comment.GetAllForIssue(settings.GitSettings.OwnerName,
                settings.GitSettings.RepositoryName,
                gitHubIssue.Number)
        ];
    }

    public Task<bool> JiraIssueExistsAsync(GitHubIssueIdentifier gitHubIssueIdentifier,
        out Atlassian.Jira.Issue jiraIssue)
    {
        return CompositeIssueResolver.JiraIssueExistsAsync(gitHubIssueIdentifier, out jiraIssue);
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
                if (repliers.Contains(c.User.Login))
                {
                    comment = c;
                    return Task.FromResult(true);
                }
            }
        }

        comment = null;
        return Task.FromResult(false);
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

    public async Task<Atlassian.Jira.Issue> CreateJiraIssueAsync(ICompositeIssue compositeIssue, string? githubComment = null)
    {
        if (compositeIssue.JiraIssue != null)
        {
            return compositeIssue.JiraIssue;
        }

        Jira jira = JiraClientProvider.GetJiraClient();
        Atlassian.Jira.Issue jiraIssue = new Atlassian.Jira.Issue(jira, ProjectKey)
        {
            Summary = compositeIssue.GitHubIssue.Title,
            Description = $"{compositeIssue.GitHubIssue.HtmlUrl}\r\n{compositeIssue.GitHubIssue.Body}",
            Type = GetIssueType(DefaultIssueType),
            Components = { GetComponent(DefaultComponent) }
        };
        string jiraId = await jira.Issues.CreateIssueAsync(jiraIssue);

        compositeIssue.JiraIssue = await GetJiraIssueAsync(jiraId);
        /* Resource not available through personal access token https://docs.github.com/en/rest/issues/comments?apiVersion=2022-11-28#create-an-issue-comment
        if (!string.IsNullOrEmpty(githubComment))
        {
            compositeIssue.Reply = await AddGithubCommentAsync(compositeIssue, $"{githubComment}\r\n{jiraId}");
        }
        */
        return compositeIssue.JiraIssue;
    }

    public async Task<IssueComment> AddGithubCommentAsync(ICompositeIssue compositeIssue, string githubComment)
    {
        GitHubClient gitHubClient = GitHubClientProvider.GetGitHubClient();
        return await gitHubClient.Issue.Comment.Create(compositeIssue.GitHubIssueIdentifier.Owner,
            compositeIssue.GitHubIssueIdentifier.RepoName, compositeIssue.GitHubIssue.Number, githubComment);
    }

    public async Task<Atlassian.Jira.Issue> GetJiraIssueAsync(string jiraId)
    {
        Jira jira = JiraClientProvider.GetJiraClient();
        return await jira.Issues.GetIssueAsync(jiraId);
    }
    
    public void DeleteMe()
    {
        Jira jira = JiraClientProvider.GetJiraClient();
        List<ProjectComponent> issueTypes = jira.Components.GetComponentsAsync("OKTA").Result.ToList();
        foreach (ProjectComponent projectComponent in issueTypes)
        {
            Message.PrintLine($"({projectComponent.Id}) {projectComponent.Name}");
        }
        
        Atlassian.Jira.Issue issue = new Atlassian.Jira.Issue(jira, ProjectKey)
        {
            Summary = "this is a test",
            Description = "The description of this test Jira",
            Type = GetIssueType(DefaultIssueType),
            Components = { GetComponent(DefaultComponent) }
        };

        string result = jira.Issues.CreateIssueAsync(issue).Result;
        Message.PrintLine(result);
    }

    protected IssueType? GetIssueType(string issueTypeName)
    {
        return GetIssueTypes().FirstOrDefault(it => it.Name.Equals(issueTypeName));
    }

    private IEnumerable<IssueType> _issueTypes;
    protected IEnumerable<IssueType> GetIssueTypes()
    {
        if (_issueTypes == null)
        {
         
            Jira jira = JiraClientProvider.GetJiraClient();
            
            _issueTypes = jira.IssueTypes.GetIssueTypesForProjectAsync(ProjectKey).Result;
        }

        return _issueTypes;
    }

    protected ProjectComponent? GetComponent(string componentName)
    {
        return GetComponents().FirstOrDefault(c => c.Name.Equals(componentName));
    }
    
    private IEnumerable<ProjectComponent> _projectComponents;
    protected IEnumerable<ProjectComponent> GetComponents()
    {
        if (_projectComponents == null)
        {
            Jira jira = JiraClientProvider.GetJiraClient();

            _projectComponents = jira.Components.GetComponentsAsync(ProjectKey).Result;
        }

        return _projectComponents;
    }
}