using GitJira.Interfaces;

namespace GitJira.Classes;

// THIS CONCEPT IS FOR FUTURE USE
public class SettingsGitAuthTokenProvider : IGitAuthTokenProvider
{
    public string GetGitAuthToken()
    {
        return Settings.Current.GitSettings.AuthToken;
    }
}