using GitJira.Interfaces;

namespace GitJira.Classes;

public class SetJiraIssueStatusResult
{
    public ICompositeIssue CompositeIssue { get; set; }
    public bool Success { get; set; }
    public Exception Exception { get; set; }
    public string Message { get; set; }
}