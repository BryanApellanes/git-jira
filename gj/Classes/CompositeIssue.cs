using Bam;
using GitJira.Interfaces;
using Octokit;

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
            if (_replyStatus == ReplyStatus.Unknown && CompositeClient != null)
            {
                if (CompositeClient.HasReplyAsync(GitHubIssue, out Octokit.IssueComment? comment).Result)
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
        set
        {
            _replyStatus = value;
        }
    }
    public ICompositeClient CompositeClient { get; set; }
    public Atlassian.Jira.Issue JiraIssue { get; set; }
    public Octokit.Issue GitHubIssue { get; set; }
    public IssueComment Reply { get; set; }

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

    public async Task<ICompositeIssue> LoadAsync(ICompositeClient compositeClient)
    {
        this.JiraIssue = await compositeClient.GetJiraIssueAsync(this.JiraKey);
        this.GitHubIssue = await compositeClient.GetGitHubIssueAsync(this.GitHubIssueIdentifier);
        return this;
    }

    public override string ToString()
    {
        return $"{GitHubIssue.CreatedAt}\t [{GitHubIssue.Number}] {GitHubIssue.Title} (GitReply: {GetReplyStatusText()}) (Jira: {JiraKey.Or("NA")})";
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