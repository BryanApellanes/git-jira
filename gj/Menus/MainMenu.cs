using Bam;
using Bam.CommandLine;
using Bam.Console;
using Bam.Shell;
using GitJira.Classes;
using GitJira.Interfaces;

namespace GitJira.Menus;

[Menu("Options")]
public class MainMenu : MenuContainer
{
    private static Settings Current;
    [MenuItem("Print settings")]
    public async Task PrintSettings()
    {
        Current ??= Settings.Load();
        Message.Print(Current.ToYaml());
    }

    [MenuItem("Set Jira UserName")]
    public async Task SetJiraUserName()
    {
        Current ??= Settings.Load();
        
        string userName = Prompt.Show("Enter user name");
        Current.JiraSettings.Credentials.UserName = userName;
        Current.SaveEncrypted();
        PrintSettings();
    }
    
    [MenuItem("Set Jira Token")]
    public async Task ShowSettings()
    {
        Current ??= Settings.Load();
        
        string token = Prompt.Show("Enter token");
        Current.JiraSettings.Credentials.Password = token;
        Current.SaveEncrypted();
        PrintSettings();
    }

    [MenuItem("Set subdomain")]
    public async Task SetSubdomain()
    {
        Current ??= Settings.Load();
        
        string subdomain = Prompt.Show("Enter subdomain");
        Current.JiraSettings.Url = $"https://{subdomain}.atlassian.net";
        Current.SaveEncrypted();
        PrintSettings();
    }

    [MenuItem("Set GitHub Auth token")]
    public async Task SetGitHubAuthToken()
    {
        Current ??= Settings.Load();
        
        string authToken = Prompt.Show("Enter auth token");
        Current.GitSettings.AuthToken = authToken;
        Current.SaveEncrypted();
        PrintSettings();
    }
    
    [MenuItem("Set GitHub Owner")]
    public async Task SetGitHubOwner()
    {
        Current ??= Settings.Load();
        
        string owner = Prompt.Show("Enter GitHub Owner");
        Current.GitSettings.Owner = owner;
        Current.SaveEncrypted();
        PrintSettings();
    }
    
    [MenuItem("Set GitHub Repo")]
    public async Task SetGitHubRepo()
    {
        Current ??= Settings.Load();
        
        string repo = Prompt.Show("Enter GitHub Repo");
        Current.GitSettings.Repository = repo;
        Current.SaveEncrypted();
        PrintSettings();
    }
}