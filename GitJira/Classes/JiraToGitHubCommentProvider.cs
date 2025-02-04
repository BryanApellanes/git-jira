using Atlassian.Jira;
using GitJira.Interfaces;

namespace GitJira.Classes;

public class JiraToGitHubCommentProvider : IJiraToGitHubCommentProvider
{
    public JiraToGitHubCommentProvider(IJiraClientProvider jiraClientProvider)
    {
        this.JiraClientProvider = jiraClientProvider;
    }
    
    protected IJiraClientProvider JiraClientProvider { get; init; }
    
    public virtual async Task<string> GetClosureCommentsAsync(ICompositeIssue compositeIssue)
    {
        Jira jira = JiraClientProvider.GetJiraClient();
        Comment[] comments = (await jira.Issues.GetCommentsAsync(compositeIssue.JiraIssue?.Key.ToString())).ToArray();
        if (comments.Length == 0)
        {
            return "NO COMMENT PROVIDED";
        }
        
        Comment comment = comments[^1];

        return $"From {comment.AuthorUser.DisplayName ?? "UNKNOWN USER"}:\r\n\r\n" +
               $"{comment.Body}";
    }
}