using GitJira.Interfaces;
using GitJira.Interfaces;

namespace GitJira.Classes;

public class GitSettingsProvider : IGitSettingsProvider
{
    public GitSettingsProvider(IGitAuthTokenProvider authTokenProvider)
    {
        this.AuthTokenProvider = authTokenProvider;
    }
    
    protected IGitAuthTokenProvider AuthTokenProvider { get; init; }
    
    public IGitSettings GetGitSettings()
    {
        return Settings.Current.GitSettings;
    }
}