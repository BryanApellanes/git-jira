using GitJira.Interfaces;

namespace GitJira.Classes;

public class CompositeIssue : ICompositeIssue
{
    public CompositeIssue(Atlassian.Jira.Issue jiraIssue, Octokit.Issue gitHubIssue)
    {
        this.JiraIssue = jiraIssue;
        this.GitHubIssue = gitHubIssue;
    }
    
    public Atlassian.Jira.Issue JiraIssue { get; set; }
    public Octokit.Issue GitHubIssue { get; set; }
    
    public string JiraId { get; set; }
    
    public GitHubIssueIdentifier GitHubIssueId { get; set; }
    
    public Task<ICompositeIssue> AddCommentAsync(string comment)
    {
        throw new NotImplementedException();
    }

    public Task<ICompositeIssue> AddJiraCommentAsync(string comment)
    {
        throw new NotImplementedException();
    }

    public Task<ICompositeIssue> AddGitHubCommentAsync(string comment)
    {
        throw new NotImplementedException();
    }

    public async Task<ICompositeIssue> LoadAsync(ICompositeClient compositeClient)
    {
        this.JiraIssue = await compositeClient.GetJiraIssueAsync(this.JiraId);
        this.GitHubIssue = await compositeClient.GetGitHubIssueAsync(this.GitHubIssueId);
        return this;
    }
}