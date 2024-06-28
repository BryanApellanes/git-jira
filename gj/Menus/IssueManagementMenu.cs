using System.Text;
using Amazon.Runtime.EventStreams;
using Bam;
using Bam.Console;
using Bam.Encryption;
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
        PrintRepositoryName();
        foreach (ICompositeIssue issue in await issueLoadTask)
        {
            PrintIssue(issue);
        }
    }
    
    [MenuItem("Select Issue")]
    public async Task SelectIssueAsync()
    {
        await issueLoadTask;
        currentIssue = Prompt.SelectFrom(issues);
    }
    
    [MenuItem("Show selected issue")]
    public void ShowSelectedIssue()
    {
        PrintIssue(currentIssue);
    }
    
    
    [MenuItem("Create Jira and reply to Github issue")]
    public async Task CreateJiraAndReplyToGithubIssue()
    {
        if (currentIssue == null)
        {
            await SelectIssueAsync();
        }
        
        Message.PrintLine("Creating Jira for GH issue: {0}", ConsoleColor.Cyan, currentIssue.GitHubIssue.Title);
        //string comment = SelectResponse();
        Atlassian.Jira.Issue jiraIssue = await client.CreateJiraIssueAsync(currentIssue);
        Message.PrintLine("Created Jira: {0}", jiraIssue.Key);
    }

    [MenuItem("Test select repsonse")]
    public async Task TestSelectResponse()
    {
        Message.PrintLine(SelectResponse());
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

    private static string SelectResponse()
    {
        List<string> responses =
        [
            "enter custom response"
        ];
        responses.AddRange(Settings.Current.CannedResponses);
        return Prompt.SelectFrom(responses);
    }
    
    private static void PrintRepositoryName()
    {
        Message.PrintLine(Settings.Current.GitSettings.RepositoryName);
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