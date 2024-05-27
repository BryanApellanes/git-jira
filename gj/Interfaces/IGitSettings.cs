namespace GitJira.Interfaces;

public interface IGitSettings
{
    string Owner { get; }
    string Repository { get; }
    string ProductHeader { get; }
    string AuthToken { get; }
}