using GitJira;
using GitJira.Classes;

namespace GitJira;

public interface IJiraSettings
{
    string Url { get; }
    JiraCredentials Credentials { get; }
}