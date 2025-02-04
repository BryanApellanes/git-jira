namespace GitJira.Interfaces;

public interface IGitHubClientProvider
{
    Octokit.GitHubClient GetGitHubClient();
}