using GitJira.Classes;

namespace GitJira;

public interface IJiraSettings
{
    string Url { get; }
    JiraCredentials Credentials { get; }
    string ProjectKey { get; }
    string IssueType { get; }
    string Component { get; }
    string ParentKey { get; }
}