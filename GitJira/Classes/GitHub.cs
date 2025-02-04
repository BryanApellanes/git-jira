using GitJira.Interfaces;

namespace GitJira.Classes;

public class GitHub
{
    public GitHub(IGitHubClientProvider gitHubClientProvider)
    {
        this.GitHubClientProvider = gitHubClientProvider;
    }
    
    protected IGitHubClientProvider GitHubClientProvider { get; init; }
}