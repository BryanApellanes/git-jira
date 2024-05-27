using GitJira.Interfaces;
using Octokit;

namespace GitJira.Classes;

public class GitHubClientProvider : IGitHubClientProvider
{
    public GitHubClientProvider(IGitSettingsProvider settingsProvider)
    {
        this.GitSettingsProvider = settingsProvider;
    }
    
    protected IGitSettingsProvider GitSettingsProvider { get; init; }
    
    public GitHubClient GetGitHubClient()
    {
        IGitSettings gitSettings = GitSettingsProvider.GetGitSettings();
        GitHubClient client = new GitHubClient(new ProductHeaderValue(gitSettings.ProductHeader))
        {
            Credentials = new Credentials(gitSettings.AuthToken)
        };
        return client;
    }
}