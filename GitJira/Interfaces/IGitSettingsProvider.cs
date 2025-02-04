using GitJira.Interfaces;

namespace GitJira.Interfaces;

public interface IGitSettingsProvider
{
    IGitSettings GetGitSettings();
    IGitSettings GetGitSettings(bool refreshAuthToken);
}