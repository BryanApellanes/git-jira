namespace GitJira.Interfaces;

public interface IGitSettings
{
    string OrgName { get; }
    string UserName { get; }
    string RepositoryName { get; }
    string ProductHeader { get; }
    string AuthToken { get; }
}