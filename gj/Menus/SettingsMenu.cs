using Bam;
using Bam.CommandLine;
using Bam.Console;
using Bam.Shell;
using GitJira.Classes;
using GitJira.Interfaces;

namespace GitJira.Menus;

[Menu("Settings")]
public class SettingsMenu : MenuContainer
{
    [MenuItem("Print settings")]
    public async Task PrintSettings()
    {
        Settings.Current ??= Settings.Load();
        Message.Print(Settings.Current.ToYaml());
    }

    [MenuItem("Set Jira UserName")]
    public async Task SetJiraUserName()
    {
        Settings.Current ??= Settings.Load();
        
        string userName = Prompt.Show("Enter user name");
        Settings.Current.JiraSettings.Credentials.UserName = userName;
        Settings.Current.SaveEncrypted();
        PrintSettings();
    }
    
    [MenuItem("Set Jira Token")]
    public async Task ShowSettings()
    {
        Settings.Current ??= Settings.Load();
        
        string token = Prompt.Show("Enter token");
        Settings.Current.JiraSettings.Credentials.Password = token;
        Settings.Current.SaveEncrypted();
        PrintSettings();
    }

    [MenuItem("Set subdomain")]
    public async Task SetSubdomain()
    {
        Settings.Current ??= Settings.Load();
        
        string subdomain = Prompt.Show("Enter subdomain");
        Settings.Current.JiraSettings.Url = $"https://{subdomain}.atlassian.net";
        Settings.Current.SaveEncrypted();
        PrintSettings();
    }

    [MenuItem("Set GitHub Auth token")]
    public async Task SetGitHubAuthToken()
    {
        Settings.Current ??= Settings.Load();
        
        string authToken = Prompt.Show("Enter auth token");
        Settings.Current.GitSettings.AuthToken = authToken;
        Settings.Current.SaveEncrypted();
        PrintSettings();
    }

    [MenuItem("Set GitHub Org")]
    public async Task SetGitHubOrg()
    {
        Settings.Current ??= Settings.Load();
        
        string org = Prompt.Show("Enter GitHub Org");
        Settings.Current.GitSettings.OrgName = org;
        Settings.Current.SaveEncrypted();
        PrintSettings();
    }
    
    [MenuItem("Set GitHub Owner")]
    public async Task SetGitHubOwner()
    {
        Settings.Current ??= Settings.Load();
        
        string owner = Prompt.Show("Enter GitHub Owner");
        Settings.Current.GitSettings.UserName = owner;
        Settings.Current.SaveEncrypted();
        PrintSettings();
    }
    
    [MenuItem("Set GitHub Repo")]
    public async Task SetGitHubRepo()
    {
        Settings.Current ??= Settings.Load();
        
        string repo = Prompt.Show("Enter GitHub Repo");
        Settings.Current.GitSettings.RepositoryName = repo;
        Settings.Current.SaveEncrypted();
        PrintSettings();
    }

    [MenuItem("Set product header")]
    public async Task ShowCurrentDirectory()
    {
        Settings.Current ??= Settings.Load();

        string header = Prompt.Show("Enter product header");
        Settings.Current.GitSettings.ProductHeader = header;
        Settings.Current.SaveEncrypted();
        PrintSettings();
    }
}