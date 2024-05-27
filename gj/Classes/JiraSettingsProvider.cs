using GitJira;
using GitJira.Interfaces;

namespace GitJira.Classes;

public class JiraSettingsProvider : IJiraSettingsProvider
{
    public JiraSettingsProvider(IJiraCredentialProvider credentialProvider)
    {
        this.JiraCredentialProvider = credentialProvider;
    }
    
    public IJiraCredentialProvider JiraCredentialProvider { get; }
    
    public IJiraSettings GetJiraSettings()
    {
        throw new NotImplementedException();
    }
}