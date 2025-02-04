using GitJira;

namespace GitJira.Interfaces;

public interface IJiraSettingsProvider
{
    IJiraSettings GetJiraSettings();
}