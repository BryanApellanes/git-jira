namespace GitJira.Interfaces;

public interface IGitSettings
{
    string OwnerName { get; }
    string UserName { get; }
    string DefaultRepositoryName { get; }
    
    public string[] RepositoryNames { get; set; }
    string ProductHeader { get; }
    string AuthToken { get; set; }
    
    public string AuthTokenCommand { get; set; }
    public string AuthTokenScope { get; set; }
}