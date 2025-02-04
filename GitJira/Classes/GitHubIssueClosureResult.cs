using GitJira.Interfaces;

namespace GitJira.Classes;

public class GitHubIssueClosureResult
{
    public ICompositeIssue CompositeIssue { get; set; }
    public bool Success { get; set; }
    public string Message { get; set; }
    public Exception Exception { get; set; }
}