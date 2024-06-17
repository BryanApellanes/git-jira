using GitJira.Classes;

namespace GitJira.Interfaces;

public interface ICompositeIssue
{
    ReplyStatus ReplyStatus { get; set; }
    Atlassian.Jira.Issue JiraIssue { get; set; }
    Octokit.Issue GitHubIssue { get; set; }
    Octokit.IssueComment Reply { get; set; }
    string JiraKey { get; set; }
    GitHubIssueIdentifier GitHubIssueIdentifier { get; set; }
    Task<ICompositeIssue> LoadAsync(ICompositeClient compositeClient);
}