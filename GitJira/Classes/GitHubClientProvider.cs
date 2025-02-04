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


    private GitHubClient? _gitHubClient;
    public GitHubClient GetGitHubClient()
    {
        if (_gitHubClient == null)
        {
            IGitSettings gitSettings = GitSettingsProvider.GetGitSettings(true);
            
            GitHubClient client = new GitHubClient(new ProductHeaderValue(gitSettings.ProductHeader))
            {
                Credentials = new Credentials(gitSettings.AuthToken)
            };
            _gitHubClient = client;
        }
        return _gitHubClient;
    }
}