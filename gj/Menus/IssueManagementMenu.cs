using Bam;
using Bam.Console;
using Bam.Shell;
using GitJira.Classes;
using GitJira.Interfaces;
using Octokit;

namespace GitJira.Menus;

[Menu("Issue Management")]
public class IssueManagementMenu : MenuContainer
{
    private static ICompositeClient client;
    private static List<ICompositeIssue> issues;
    private static ICompositeIssue currentIssue;

    private static Task<List<ICompositeIssue>> issueLoadTask;

    static IssueManagementMenu()
    {
        client = GitJiraServiceRegistry.GetRegistry().Get<ICompositeClient>();
        issues = new List<ICompositeIssue>();
        issueLoadTask = GetIssues(PrintIssue);  
    }

    [MenuItem("Show issues")]
    public async Task ShowIssues()
    {
        foreach (ICompositeIssue issue in await issueLoadTask)
        {
            PrintIssue(issue);
        }
    }
    
    [MenuItem("Select Issue")]
    public async Task SelectIssue()
    {
        await issueLoadTask;
        currentIssue = Prompt.SelectFrom(issues);
    }
    
    [MenuItem("Show selected issue")]
    public void ShowSelectedIssue()
    {
        PrintIssue(currentIssue);
    }

    [MenuItem("print settings with repsonses")]
    public void PrintSettings()
    {
        Settings settings = Settings.Load();
        settings.CannedResponses = new Dictionary<string, string>()
        {
            { 
                "Opening", "Thanks for opening this issue.  I've filed an internal ticket for review and prioritization. {0}"
            },
            {
                "Reporting", "Thanks for reporting this issue.  I've filed an internal ticket for review and prioritization. {0}"
            }
        };
        
        Message.PrintLine(settings.ToYaml());
    }
    
    [MenuItem("Create Jira and reply to Github issue")]
    public async Task CreateJiraAndReplyToGithubIssue()
    {
        if (currentIssue == null)
        {
            SelectIssue();
        }
        
        Message.PrintLine("Creating Jira for GH issue: {0}", ConsoleColor.Cyan, currentIssue.GitHubIssue.Title);

        //Atlassian.Jira.Issue jiraIssue = await client.CreateJiraIssueAsync(currentIssue);
    }
    
    public static async Task<List<ICompositeIssue>> GetIssues(Action<ICompositeIssue> onIssueLoaded = null)
    {
        Settings.Current ??= Settings.Load();

        if (!issues.Any())
        {
            Message.PrintLine("Loading composite issues...", ConsoleColor.Yellow);
            LoadCompositeIssues(onIssueLoaded);
        }

        return issues;
    }
    
    public static void PrintIssue(ICompositeIssue issue)
    {
        if (issue == null)
        {
            Message.PrintLine("No issue selected", ConsoleColor.Red);
            return;
        }
        Message.PrintLine($"- {issue.ToString()})");
    }
    
    private static void LoadCompositeIssues(Action<ICompositeIssue> onIssueLoaded = null)
    {
        foreach (ICompositeIssue issue in client.GetCompositeIssues(Settings.Current.GitSettings.GetOrgRepoIdentifier()))
        {;
            issues.Add(issue);
            if (onIssueLoaded != null)
            {
                onIssueLoaded(issue);
            }
        }
    }
}