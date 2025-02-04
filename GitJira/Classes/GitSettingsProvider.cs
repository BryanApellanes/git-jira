using GitJira.Interfaces;
using GitJira.Interfaces;
using Google.Protobuf;

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

    public IGitSettings GetGitSettings(bool refreshAuthToken)
    {
        if (!refreshAuthToken)
        {
            return GetGitSettings();
        }
        IGitSettings gitSettings = GetGitSettings();
        gitSettings.AuthToken = AuthTokenProvider.GetGitAuthToken();
        Settings.Current.GitSettings = (GitSettings)gitSettings;
        Settings.Current.Save();
        return gitSettings;
    }
}