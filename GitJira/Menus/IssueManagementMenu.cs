using Atlassian.Jira;
using Bam.Console;
using Bam.Shell;
using GitJira.Classes;
using GitJira.Interfaces;
using Octokit;

namespace GitJira.Menus;

[Menu("Issue Management")]
public class IssueManagementMenu : MenuContainer
{
    private static ICompositeIssueManager _issueManager;
    private static ICompositeIssue? _currentIssue;
    private static string _currentRepositoryName;

    private static Dictionary<string, List<ICompositeIssue>> _issuesByRepo;
    private static Task<Dictionary<string, List<ICompositeIssue>>> _issuesByRepoLoadTask;

    static IssueManagementMenu()
    {
        Reset();
    }

    public static void Reset()
    {
        _issueManager = GitJiraServiceRegistry.GetRegistry().Get<ICompositeIssueManager>();
        _currentRepositoryName = Settings.Current.GitSettings.DefaultRepositoryName;

        _issuesByRepo = new Dictionary<string, List<ICompositeIssue>>();
    }

    [MenuItem("(Re)Load issues")]
    public async Task ReloadIssues()
    {
        _issuesByRepo.Clear();
        _issuesByRepoLoadTask = LoadIssuesForAllReposAsync();
    }
    
    [MenuItem("Select repo")]
    public async Task SelectRepo()
    {
        Settings.Current ??= Settings.Load();
        PrintRepositoryName();
        Console.Clear();
        _currentRepositoryName = Settings.Current.GitSettings.RepositoryNames[Prompt.SelectFrom(Settings.Current.GitSettings.RepositoryNames)];
        PrintRepositoryName();
    }
    
    [MenuItem("Show issues for current repo")]
    public async Task ShowIssuesForCurrentRepo()
    {
        Console.Clear();
        PrintRepositoryName();
        foreach (ICompositeIssue issue in (await _issuesByRepoLoadTask)[_currentRepositoryName])
        {
            PrintIssue(issue);
        }
    }
    
    [MenuItem("Select Issue")]
    public async Task<ICompositeIssue> SelectIssueAsync()
    {   
        await _issuesByRepoLoadTask;
        Console.Clear();
        _currentIssue = Prompt.SelectFrom(await GetIssueListAsync(), color: ConsoleColor.Cyan);
        return _currentIssue;
    }

    [MenuItem("Show issue description")]
    public void ShowIssueDescription()
    {
        PrintDescription(_currentIssue);
    }
    
    [MenuItem("Show issue comments")]
    public void ShowIssueComments()
    {
        PrintIssue(_currentIssue);
        PrintGitHubComments(_currentIssue);
        PrintJiraComments(_currentIssue);
    }

    [MenuItem("Create Jira and reply to GitHub issue")]
    public async Task CreateJiraAndReplyToGitHubIssue()
    {
        if (_currentIssue == null)
        {
            await SelectIssueAsync();
        }
        
        string comment = SelectResponse();
        
        Message.PrintLine("Creating Jira for GH issue: {0}", ConsoleColor.Cyan, _currentIssue.GitHubIssue.Title);
        Atlassian.Jira.Issue jiraIssue = await _issueManager.CreateJiraIssueAsync(_currentIssue, comment);
        Message.PrintLine("Jira: {0}", jiraIssue.Key);
    }

    [MenuItem("Add GitHub comment")]
    public async Task AddGitHubComment()
    {
        if (_currentIssue == null)
        {
            _currentIssue = await SelectIssueAsync();
        }

        IssueComment comment = await _issueManager.AddGithubCommentAsync(_currentIssue, Prompt.Show("Type your comment"));
        PrintIssue(_currentIssue);
        PrintGitHubComments(_currentIssue);
    }

    [MenuItem("Close GitHub issue with comment")]
    public async Task CloseGitHubIssueWithComment()
    {
        if (_currentIssue == null)
        {
            _currentIssue = await SelectIssueAsync();
        }

        string comment = Prompt.Show("Type your comment");
        if (string.IsNullOrEmpty(comment))
        {
            Message.PrintLine("No comment entered", ConsoleColor.Magenta);
            return;
        }

        await _issueManager.CloseGithubIssueWithCommentAsync(_currentIssue, comment);
    }

    [MenuItem("Add Jira comment")]
    public async Task AddJiraComment()
    {
        if (_currentIssue == null)
        {
            _currentIssue = await SelectIssueAsync();
        }
        
        string comment = Prompt.Show("Type your comment");
        if (string.IsNullOrEmpty(comment))
        {
            Message.PrintLine("No comment entered", ConsoleColor.Magenta);
            return;
        }

        await _issueManager.AddJiraCommentAsync(_currentIssue, comment);
        _currentIssue = await ReloadCompositeIssueAsync(_currentIssue);
    }
    
    [MenuItem("Close or resolve Jira with comment")]
    public async Task CloseOrResolveJira()
    {
        if (_currentIssue == null)
        {
            _currentIssue = await SelectIssueAsync();
        }

        string comment = Prompt.Show("Type your comment");
        if (string.IsNullOrEmpty(comment))
        {
            Message.PrintLine("No comment entered", ConsoleColor.Magenta);
            return;
        }

        await _issueManager.AddJiraCommentAsync(_currentIssue, comment);
        await _issueManager.SetJiraIssueStatusAsync(_currentIssue, Prompt.SelectFrom<string>(new[] { "Resolve Issue", "Close Issue" }));
        _currentIssue = await ReloadCompositeIssueAsync(_currentIssue);
    }
    
    private static async Task<Dictionary<string, List<ICompositeIssue>>> LoadIssuesForAllReposAsync()
    {
        Settings.Current ??= Settings.Load();
        foreach (GitHubRepoIdentifier repoIdentifier in Settings.Current.GitSettings.GetOrgRepoIdentifiers())
        {
            LoadCompositeIssues(repoIdentifier, PrintIssue);
        }

        return _issuesByRepo;
    }

    private static async Task<List<ICompositeIssue>> GetIssueListAsync()
    {
        return (await _issuesByRepoLoadTask)[_currentRepositoryName];
    }

    private static void PrintDescription(ICompositeIssue? compositeIssue)
    {
        if (compositeIssue == null)
        {
            Message.PrintLine("No issue specified", ConsoleColor.Red);
            return;
        }   
        
        Message.PrintLine(compositeIssue.GitHubIssue.Body, ConsoleColor.Cyan);
    }
    
    private static void PrintIssue(ICompositeIssue? compositeIssue)
    {
        if (compositeIssue == null)
        {
            Message.PrintLine("No issue specified", ConsoleColor.Red);
            return;
        }

        ConsoleColor color = ConsoleColor.Cyan;
        if (compositeIssue.JiraIssueIsClosed)
        {
            color = ConsoleColor.Green;
        }
        else if(compositeIssue.ReplyStatus == ReplyStatus.NoReply)
        {
            color = ConsoleColor.Yellow;
        }

        if (compositeIssue.GitHubIssue.State == ItemState.Closed)
        {
            color = ConsoleColor.DarkRed;
        }
        Message.PrintLine($"- {compositeIssue})", color);
        if(compositeIssue.GitHubIssue.Labels.Any())
        {
            Message.PrintLine($"\tLabels: {string.Join(',', compositeIssue.GitHubIssue.Labels.Select(l=> l.Name))}", ConsoleColor.DarkYellow);
        };
    }

    private static void PrintGitHubComments(ICompositeIssue? compositeIssue)
    {
        if (compositeIssue == null)
        {
            return;
        }
        Message.PrintLine("*** GitHub Comments ***", ConsoleColor.DarkCyan);
        foreach (IssueComment comment in _issueManager.GetGitHubCommentsAsync(compositeIssue).Result)
        {
            Message.PrintLine($"{comment.CreatedAt} ({comment.User.Login})", ConsoleColor.DarkCyan);
            Message.PrintLine(comment.Body, ConsoleColor.DarkCyan);
            Message.PrintLine("****");
        }
    }

    private static void PrintJiraComments(ICompositeIssue? compositeIssue)
    {
        if (compositeIssue == null)
        {
            return;
        }
        Message.PrintLine("*** Jira Comments ***", ConsoleColor.DarkYellow);
        foreach (Comment comment in _issueManager.GetJiraCommentsAsync(compositeIssue).Result)
        {
            Message.PrintLine($"{comment.CreatedDate} ({comment.AuthorUser.Email})", ConsoleColor.Yellow);
            Message.PrintLine(comment.Body, ConsoleColor.Yellow);
            Message.PrintLine("****", ConsoleColor.DarkYellow);
            Message.PrintLine();
        }
    }
    
    private static string SelectResponse()
    {
        List<string> responses = new List<string>();
        responses.AddRange(Settings.Current.CannedResponses);
        return Prompt.SelectFrom(responses, color: ConsoleColor.Cyan);
    }
    
    private static void PrintRepositoryName()
    {
        Message.PrintLine(_currentRepositoryName, ConsoleColor.Yellow);
        Message.PrintLine();
    }
    
    private static void LoadCompositeIssues(GitHubRepoIdentifier gitHubRepoIdentifier, Action<ICompositeIssue>? onIssueLoaded = null)
    {
        if (!_issuesByRepo.ContainsKey(gitHubRepoIdentifier.RepositoryName))
        {
            _issuesByRepo.Add(gitHubRepoIdentifier.RepositoryName, new List<ICompositeIssue>());
        }
        foreach (ICompositeIssue issue in _issueManager.GetCompositeIssues(gitHubRepoIdentifier))
        {
            _issuesByRepo[gitHubRepoIdentifier.RepositoryName].Add(issue);
            onIssueLoaded?.Invoke(issue);
        }
    }

    private static async Task<ICompositeIssue> ReloadCompositeIssueAsync(ICompositeIssue compositeIssue)
    {
        if (_issuesByRepo.ContainsKey(compositeIssue.GitHubIssueIdentifier.RepoName))
        {
            if (_issuesByRepo[compositeIssue.GitHubIssueIdentifier.RepoName].Contains(compositeIssue))
            {
                _issuesByRepo[compositeIssue.GitHubIssueIdentifier.RepoName].Remove(compositeIssue);
            }

            ICompositeIssue reloaded = await _issueManager.ReloadCompositeIssueAsync(compositeIssue);
            _issuesByRepo[compositeIssue.GitHubIssueIdentifier.RepoName].Add(reloaded);
            return reloaded;
        }

        return compositeIssue;
    }
}