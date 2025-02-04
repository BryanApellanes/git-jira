
namespace GitJira.Classes;

public class GitHubIssueIdentifier
{
    public GitHubIssueIdentifier()
    {
        
    }

    public GitHubIssueIdentifier(Settings settings, int githubIssueNumber)
    {
        this.Owner = settings.GitSettings.OwnerName;
        this.RepoName = settings.GitSettings.DefaultRepositoryName;
        this.IssueNumber = githubIssueNumber;
    }
    public string Owner { get; set; }
    public string RepoName { get; set; }
    public int IssueNumber { get; set; }
}