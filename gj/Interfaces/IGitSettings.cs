namespace GitJira.Interfaces;

public interface IGitSettings
{
    string OwnerName { get; }
    string UserName { get; }
    string RepositoryName { get; }
    string ProductHeader { get; }
    string AuthToken { get; }
}