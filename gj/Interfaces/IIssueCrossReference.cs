namespace GitJira.Interfaces;

public interface IIssueCrossReference
{
    string JiraId { get; set; }
    string GitHubOwner { get; set; }
    string GitHubRepo { get; set; }
    int GitHubIssueNumber { get; set; }
    IIssueCrossReference AddComment(string comment);
}