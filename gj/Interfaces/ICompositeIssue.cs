using GitJira.Classes;

namespace GitJira.Interfaces;

public interface ICompositeIssue
{
    Atlassian.Jira.Issue JiraIssue { get; set; }
    Octokit.Issue GitHubIssue { get; set; }
    string JiraId { get; set; }
    GitHubIssueIdentifier GitHubIssueId { get; set; }
    Task<ICompositeIssue> AddCommentAsync(string comment);
    Task<ICompositeIssue> AddJiraCommentAsync(string comment);
    Task<ICompositeIssue> AddGitHubCommentAsync(string comment);
    Task<ICompositeIssue> LoadAsync(ICompositeClient compositeClient);
}