using Atlassian.Jira;
using Bam;
using GitJira.Interfaces;
using Octokit;
using Issue = Atlassian.Jira.Issue;

namespace GitJira.Classes;

public class JiraReferenceResolver : IJiraReferenceResolver
{
    public const string Prefix = "OKTA-";
    public static string Pattern = $"{Prefix}XXXXXX";
    
    public JiraReferenceResolver(IJiraClientProvider jiraClientProvider, IGitHubClientProvider gitHubClientProvider)
    {
        this.JiraClientProvider = jiraClientProvider;
        this.GitHubClientProvider = gitHubClientProvider;
    }
    
    protected IJiraClientProvider JiraClientProvider { get; init; }
    protected IGitHubClientProvider GitHubClientProvider { get; init; }
    
    public Task<bool> JiraReferenceExistsInCommentsAsync(GitHubIssueIdentifier gitHubIssueIdentifier, out Issue? jiraIssue)
    {
        GitHubClient gitHubClient = GitHubClientProvider.GetGitHubClient();
        IReadOnlyList<IssueComment> comments = gitHubClient
            .Issue
            .Comment
            .GetAllForIssue(gitHubIssueIdentifier.Owner, gitHubIssueIdentifier.RepoName, gitHubIssueIdentifier.IssueNumber)
            .Result;

        foreach (IssueComment comment in comments)
        {
            if (ContainsJiraId(comment.Body, out string jiraId))
            {
                Jira jira = JiraClientProvider.GetJiraClient();
                jiraIssue = jira.Issues.GetIssueAsync(jiraId).Result;
                return Task.FromResult(true);
            }
        }

        jiraIssue = null;
        return Task.FromResult(false);
    }
    
    public Task<bool> JiraReferenceLabelExistsAsync(GitHubIssueIdentifier gitHubIssueIdentifier, out Issue? jiraIssue)
    {
        GitHubClient gitHubClient = GitHubClientProvider.GetGitHubClient();
        IReadOnlyList<Label> labels = gitHubClient
            .Issue
            .Labels
            .GetAllForIssue(gitHubIssueIdentifier.Owner, gitHubIssueIdentifier.RepoName,
            gitHubIssueIdentifier.IssueNumber)
            .Result;

        foreach (Label label in labels)
        {
            if (IsJiraId(label.Name))
            {
                Jira jira = JiraClientProvider.GetJiraClient();
                jiraIssue = jira.Issues.GetIssueAsync(label.Name).Result;
                return Task.FromResult(true);
            }
        }
        
        jiraIssue = null;
        return Task.FromResult(false);
    }

    public virtual bool ContainsJiraId(string body, out string jiraId)
    {
        string[] segments = body.Split('\n', '\r', ';', '(', ')', '.', ':', ' ');

        foreach (string s in segments)
        {
            string segment = s.Trim();
            jiraId = segment;
            
            if (IsJiraId(segment)) return true;
        }

        jiraId = string.Empty;
        return false;
    }

    public static bool IsJiraId(string value)
    {
        if (value.Length == Pattern.Length && value.StartsWith(Prefix))
        {
            string nums = value.Remove(0, Prefix.Length);
            bool allAreNumbers = true;
            foreach (char num in nums)
            {
                if (!num.IsNumber())
                {
                    allAreNumbers = false;
                }
            }

            if (allAreNumbers)
            {
                return true;
            }
        }

        return false;
    }
}