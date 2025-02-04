namespace GitJira.Interfaces;

public interface IJiraToGitHubCommentProvider
{
    public Task<string> GetClosureCommentsAsync(ICompositeIssue compositeIssue);
}