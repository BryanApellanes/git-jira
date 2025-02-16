using GitJira.Interfaces;

namespace GitJira.Classes;

public class JiraSettingsProvider : IJiraSettingsProvider
{
    public JiraSettingsProvider()
    {
    }
    
    public IJiraSettings GetJiraSettings()
    {
        return Settings.Current.JiraSettings;
    }
}