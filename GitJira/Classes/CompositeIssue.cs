using Bam;
using GitJira.Interfaces;
using Octokit;
using Issue = Octokit.Issue;

namespace GitJira.Classes;

public class CompositeIssue : ICompositeIssue
{
    public CompositeIssue(Issue gitHubIssue, Atlassian.Jira.Issue jiraIssue)
    {
        this.JiraIssue = jiraIssue;
        this.GitHubIssue = gitHubIssue;
    }

    private ReplyStatus _replyStatus;

    public ReplyStatus ReplyStatus
    {
        get
        {
            if (_replyStatus == ReplyStatus.Unknown && CompositeIssueManager != null)
            {
                if (CompositeIssueManager.HasReplyAsync(GitHubIssue, out IssueComment? comment).Result)
                {
                    Reply = comment;
                    _replyStatus = ReplyStatus.ReplyExists;
                }
                else
                {
                    _replyStatus = ReplyStatus.NoReply;
                }
            }

            return _replyStatus;
        }
        set => _replyStatus = value;
    }
    public ICompositeIssueManager CompositeIssueManager { get; set; }
    public Atlassian.Jira.Issue? JiraIssue { get; set; }
    public Issue GitHubIssue { get; set; }
    public IssueComment? Reply { get; set; }
    
    public bool JiraIssueIsClosed
    {
        get
        {
            if (JiraIssue != null)
            {
                return JiraIssue.Status.Name.Equals("Closed") || JiraIssue.Status.Name.Equals("Resolved");
            }

            return false;
        }
    }
    
    private string _jiraId;
    public string JiraKey
    {
        get
        {
            if (string.IsNullOrEmpty(_jiraId) && JiraIssue != null)
            {
                return JiraIssue.Key.ToString();
            }

            return _jiraId;
        }
        set => _jiraId = value;
    }
    
    public GitHubIssueIdentifier GitHubIssueIdentifier { get; set; }

    public async Task<ICompositeIssue> LoadAsync(ICompositeIssueManager compositeIssueManager)
    {
        this.JiraIssue = await compositeIssueManager.GetJiraIssueAsync(this.JiraKey);
        this.GitHubIssue = await compositeIssueManager.GetGitHubIssueAsync(this.GitHubIssueIdentifier);
        return this;
    }

    public override string ToString()
    {
        return $"{GitHubIssue.CreatedAt}\t ({GitHubIssueIdentifier.RepoName}) ({GitHubIssue.User?.Login}) [{GitHubIssue.Number}] {GitHubIssue.Title} (GitReply: {GetReplyStatusText()}) (Jira: {JiraKey.Or("NA")}) (Jira is Closed: {JiraIssueIsClosed})";
    }

    private string GetReplyStatusText()
    {
        if (ReplyStatus == ReplyStatus.ReplyExists)
        {
            return Reply.User.Login;
        }

        return ReplyStatus.ToString();
    }
}