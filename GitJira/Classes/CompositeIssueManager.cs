using Atlassian.Jira;
using Bam.Logging;
using GitJira.Interfaces;
using Octokit;
using Issue = Octokit.Issue;

namespace GitJira.Classes;

public class CompositeIssueManager : ICompositeIssueManager
{
    public CompositeIssueManager(IJiraClientProvider jiraClientProvider, IGitHubClientProvider gitHubClientProvider, ICompositeIssueLoader compositeIssueLoader, IJiraToGitHubCommentProvider jiraToGitHubCommentProvider, ISettingsProvider settingsProvider, ILogger logger)
    {
        this.JiraClientProvider = jiraClientProvider;
        this.GitHubClientProvider = gitHubClientProvider;
        this.JiraToGitHubCommentProvider = jiraToGitHubCommentProvider;
        this.SettingsProvider = settingsProvider;
        this.CompositeIssueLoader = compositeIssueLoader;
        this.Logger = logger;
    }
    
    public IJiraClientProvider JiraClientProvider { get; init; }
    public IGitHubClientProvider GitHubClientProvider { get; init; }
    
    public IJiraToGitHubCommentProvider JiraToGitHubCommentProvider { get; init; }
    public ICompositeIssueLoader CompositeIssueLoader { get; init; }
    
    public ILogger Logger { get; init; }
    
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
            yield return this.CompositeIssueLoader.LoadCompositeIssueAsync(this, new GitHubIssueIdentifier()
            {
                Owner = gitHubRepoIdentifier.Owner,
                RepoName = gitHubRepoIdentifier.RepositoryName,
                IssueNumber = issue.Number
            }).Result;
        }
    }

    public async Task<ICompositeIssue> ReloadCompositeIssueAsync(ICompositeIssue compositeIssue)
    {
        return await this.CompositeIssueLoader.LoadCompositeIssueAsync(this, new GitHubIssueIdentifier()
        {
            Owner = compositeIssue.GitHubIssueIdentifier.Owner,
            RepoName = compositeIssue.GitHubIssueIdentifier.RepoName,
            IssueNumber = compositeIssue.GitHubIssue.Number
        });
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

    public async Task<List<IssueComment>> GetGitHubCommentsAsync(ICompositeIssue compositeIssue)
    {
        return await GetGitHubCommentsAsync(compositeIssue.GitHubIssueIdentifier);
    }
    
    public async Task<List<IssueComment>> GetGitHubCommentsAsync(GitHubIssueIdentifier gitHubIssueIdentifier)
    {
        GitHubClient github = GitHubClientProvider.GetGitHubClient();
        return
        [
            ..await github.Issue.Comment.GetAllForIssue(gitHubIssueIdentifier.Owner,
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

        IReadOnlyList<IssueComment> comments = await client.Issue.Comment.GetAllForIssue(settings.GitSettings.OwnerName,
            settings.GitSettings.DefaultRepositoryName,
            gitHubIssue.Number);
        return new List<IssueComment>(comments);
    }

    public Task<bool> JiraIssueExistsAsync(GitHubIssueIdentifier gitHubIssueIdentifier,
        out Atlassian.Jira.Issue jiraIssue)
    {
        return CompositeIssueLoader.JiraIssueExistsAsync(gitHubIssueIdentifier, out jiraIssue);
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

    /// <summary>
    /// Creates a Jira for the specified issue and adds a comment to the GitHub issue.
    /// </summary>
    /// <param name="compositeIssue"></param>
    /// <param name="githubComment"></param>
    /// <remarks>
    /// If the specified composite issue has a Jira then that issue is returned and no comment is made on the GitHub issue.
    /// If no githubComment is provided no comment is made on the GitHub issue.
    /// </remarks>
    /// <returns></returns>
    public async Task<Atlassian.Jira.Issue> CreateJiraIssueAsync(ICompositeIssue compositeIssue, string? githubComment = null)
    {
        if (compositeIssue.JiraIssue != null)
        {
            Logger.Info("Jira for the specified issue already exists ({0})", compositeIssue.JiraKey);
            return compositeIssue.JiraIssue;
        }

        Jira jira = JiraClientProvider.GetJiraClient();
        Atlassian.Jira.Issue jiraIssue = new Atlassian.Jira.Issue(jira, GetProjectKey(), GetParentKey())
        {
            Summary = compositeIssue.GitHubIssue.Title,
            Description = $"{compositeIssue.GitHubIssue.HtmlUrl}\r\n{compositeIssue.GitHubIssue.Body}",
            Type = GetIssueType(),
            Components = { GetComponent() }
        };
        jiraIssue.Labels.AddRange(compositeIssue.GitHubIssue.Labels.Select(l=> l.Name.Replace(" ", "_")));
        
        string jiraId = await jira.Issues.CreateIssueAsync(jiraIssue);
        compositeIssue.JiraIssue = await GetJiraIssueAsync(jiraId);

        if (compositeIssue.ReplyStatus != ReplyStatus.ReplyExists)
        {
            await TryAddGithubCommentWithUserTagAsync(compositeIssue, githubComment, jiraId);   
        }
        else
        {
            await TryAddGithubCommentAsync(compositeIssue, "internal tracking number: ", jiraId);
        }
        
        return compositeIssue.JiraIssue;
    }

    public Task<Comment> AddJiraCommentAsync(ICompositeIssue compositeIssue, string jiraComment)
    {
        Settings settings = SettingsProvider.GetSettings();
        Jira jira = JiraClientProvider.GetJiraClient();
        return jira.Issues.AddCommentAsync(compositeIssue.JiraKey, new Comment()
        {
            Author = settings.JiraSettings.Credentials.UserName,
            Body = jiraComment
        });
    }

    public async Task<IEnumerable<Comment>> GetJiraCommentsAsync(ICompositeIssue compositeIssue)
    {
        if (string.IsNullOrEmpty(compositeIssue.JiraKey))
        {
            return [];
        }
        Jira jira = JiraClientProvider.GetJiraClient();
        return await jira.Issues.GetCommentsAsync(compositeIssue.JiraKey);
    }


    public async Task<GitHubIssueClosureResult> CloseGithubIssueWithCommentAsync(ICompositeIssue compositeIssue, string githubComment)
    {
        await AddGithubCommentAsync(compositeIssue, githubComment);
        return await CloseGithubIssueAsync(compositeIssue);
    }
    
    public async Task<GitHubIssueClosureResult> CloseGithubIssueAsync(ICompositeIssue compositeIssue)
    {
        try
        {
            GitHubClient gitHubClient = GitHubClientProvider.GetGitHubClient();
            Issue issue = await gitHubClient
                .Issue
                .Update(compositeIssue.GitHubIssueIdentifier.Owner, compositeIssue.GitHubIssueIdentifier.RepoName,
                    compositeIssue.GitHubIssue.Number, new IssueUpdate()
                    {
                        State = ItemState.Closed
                    });
            compositeIssue.GitHubIssue = issue;
            return new GitHubIssueClosureResult()
            {
                CompositeIssue = compositeIssue,
                Success = true
            };
        }
        catch (Exception ex)
        {
            return new GitHubIssueClosureResult()
            {
                CompositeIssue = compositeIssue,
                Success = false,
                Exception = ex,
                Message = ex.Message
            };
        }
    }

    public async Task<SetJiraIssueStatusResult> SetJiraIssueStatusAsync(ICompositeIssue compositeIssue, string statusTransitionName)
    {
        try
        {
            if (compositeIssue.JiraIssue != null)
            {
                await compositeIssue.JiraIssue.WorkflowTransitionAsync(statusTransitionName);
                return new SetJiraIssueStatusResult()
                {
                    Success = true
                };
            }

            return new SetJiraIssueStatusResult()
            {
                Success = false,
                Message = "JiraIssue not specified"
            };
        }
        catch (Exception ex)
        {
            return new SetJiraIssueStatusResult()
            {
                Success = false,
                Message = ex.Message,
                Exception = ex
            };
        }
    }
    
    public async Task<string> GetJiraCommentsForGitHubIssueClosureAsync(ICompositeIssue compositeIssue)
    {
        return await JiraToGitHubCommentProvider.GetClosureCommentsAsync(compositeIssue);
    }

    private async Task TryAddGithubCommentWithUserTagAsync(ICompositeIssue compositeIssue, string? githubComment, string jiraId)
    {
        try
        {
            if (!string.IsNullOrEmpty(githubComment))
            {
                compositeIssue.Reply = await AddGithubCommentAsync(compositeIssue, $"@{compositeIssue.GitHubIssue.User.Login} {githubComment}\r\n{jiraId}");
                compositeIssue.ReplyStatus = ReplyStatus.ReplyExists;
            }
        }
        catch (Exception ex)
        {
            Logger.Error("Exception adding github comment for issue: ({0})", ex, compositeIssue?.ToString() ?? "");
        }
    }

    private async Task TryAddGithubCommentAsync(ICompositeIssue compositeIssue, string? githubComment, string jiraId)
    {
        try
        {
            if (!string.IsNullOrEmpty(githubComment))
            {
                compositeIssue.Reply = await AddGithubCommentAsync(compositeIssue, $"@{githubComment}\r\n{jiraId}");
                compositeIssue.ReplyStatus = ReplyStatus.ReplyExists;
            }
        }
        catch (Exception ex)
        {
            Logger.Error("Exception adding github comment for issue: ({0})", ex, compositeIssue?.ToString() ?? "");
        }
    }
    
    public async Task<IssueComment> AddGithubCommentAsync(ICompositeIssue compositeIssue, string githubComment)
    {
        GitHubClient gitHubClient = GitHubClientProvider.GetGitHubClient();
        IssueComment comment = await gitHubClient.Issue.Comment.Create(compositeIssue.GitHubIssueIdentifier.Owner,
            compositeIssue.GitHubIssueIdentifier.RepoName, compositeIssue.GitHubIssue.Number, githubComment);
        return comment;
    }

    public async Task<Atlassian.Jira.Issue> GetJiraIssueAsync(string jiraId)
    {
        Jira jira = JiraClientProvider.GetJiraClient();
        return await jira.Issues.GetIssueAsync(jiraId);
    }

    protected virtual string GetProjectKey()
    {
        Settings settings = SettingsProvider.GetSettings();
        return settings.JiraSettings?.ProjectKey ?? JiraSettings.DefaultProjectKey;
    }
    
    protected virtual string GetParentKey()
    {
        Settings settings = SettingsProvider.GetSettings();
        return settings.JiraSettings?.ParentKey ?? JiraSettings.DefaultParentKey;
    }

    protected virtual IssueType? GetIssueType()
    {
        Settings settings = SettingsProvider.GetSettings();
        return GetIssueType(settings.JiraSettings?.IssueType ?? JiraSettings.DefaultIssueType);
    }
    
    protected virtual IssueType? GetIssueType(string issueTypeName)
    {
        return GetIssueTypes().FirstOrDefault(it => it.Name.Equals(issueTypeName));
    }

    private IEnumerable<IssueType>? _issueTypes;
    protected IEnumerable<IssueType> GetIssueTypes()
    {
        if (_issueTypes == null)
        {
         
            Jira jira = JiraClientProvider.GetJiraClient();
            
            _issueTypes = jira.IssueTypes.GetIssueTypesForProjectAsync(GetProjectKey()).Result;
        }

        return _issueTypes;
    }

    protected virtual ProjectComponent? GetComponent()
    {
        Settings settings = SettingsProvider.GetSettings();
        return GetComponent(settings.JiraSettings?.Component ?? JiraSettings.DefaultComponent);
    }
    
    protected virtual ProjectComponent? GetComponent(string componentName)
    {
        return GetComponents().FirstOrDefault(c => c.Name.Equals(componentName));
    }
    
    private IEnumerable<ProjectComponent>? _projectComponents;
    protected IEnumerable<ProjectComponent> GetComponents()
    {
        if (_projectComponents == null)
        {
            Jira jira = JiraClientProvider.GetJiraClient();

            _projectComponents = jira.Components.GetComponentsAsync(GetProjectKey()).Result;
        }

        return _projectComponents;
    }
}