using Bam;
using Bam.CommandLine;
using Bam.Console;
using Bam.Encryption;
using Bam.Shell;
using GitJira.Classes;
using GitJira.Interfaces;

namespace GitJira.Menus;

[Menu("Settings")]
public class SettingsMenu : MenuContainer
{
    [MenuItem("Print settings")]
    public async Task PrintSettingsAsync()
    {
        Settings.Current ??= Settings.Load();
        Message.Print(Settings.Current.ToYaml());
    }

    [MenuItem("Set Jira (Atlassian) UserName")]
    public async Task SetJiraUserName()
    {
        Settings.Current ??= Settings.Load();
        
        string userName = Prompt.Show("Enter user name");
        Settings.Current.JiraSettings.Credentials.UserName = userName;
        Settings.Current.Save();
        await PrintSettingsAsync();
    }
    
    [MenuItem("Set Jira (Atlassian) Token")]
    public async Task ShowSettings()
    {
        Settings.Current ??= Settings.Load();
        
        string token = Prompt.Show("Enter token");
        if (!string.IsNullOrEmpty(token))
        {
            Settings.Current.JiraSettings.Credentials.Password = token;
            Settings.Current.Save();
        }
        await PrintSettingsAsync();
    }

    [MenuItem("Set Jira (Atlassian) subdomain")]
    public async Task SetSubdomain()
    {
        Settings.Current ??= Settings.Load();
        
        string subdomain = Prompt.Show("Enter subdomain");
        Settings.Current.JiraSettings.Url = $"https://{subdomain}.atlassian.net";
        Settings.Current.Save();
        await PrintSettingsAsync();
    }

    [MenuItem("Set GitHub Auth token")]
    public async Task SetGitHubAuthToken()
    {
        Settings.Current ??= Settings.Load();
        
        string authToken = Prompt.Show("Enter auth token");
        Settings.Current.GitSettings.AuthToken = authToken;
        Settings.Current.Save();
        IssueManagementMenu.Reset();
        await PrintSettingsAsync();
    }

    [MenuItem("Set GitHub Owner")]
    public async Task SetGitHubOrg()
    {
        Settings.Current ??= Settings.Load();
        
        string org = Prompt.Show("Enter GitHub Owner");
        if (!string.IsNullOrEmpty(org))
        {
            Settings.Current.GitSettings.OwnerName = org;
            Settings.Current.Save();   
        }
        await PrintSettingsAsync();
    }
    
    [MenuItem("Set GitHub User")]
    public async Task SetGitHubUser()
    {
        Settings.Current ??= Settings.Load();
        
        string user = Prompt.Show("Enter GitHub User");
        Settings.Current.GitSettings.UserName = user;
        Settings.Current.Save();
        await PrintSettingsAsync();
    }
    
    [MenuItem("Set GitHub Repo")]
    public async Task SetGitHubRepo()
    {
        Settings.Current ??= Settings.Load();
        
        string repo = Prompt.Show("Enter GitHub Repo");
        Settings.Current.GitSettings.DefaultRepositoryName = repo;
        Settings.Current.Save();
        await PrintSettingsAsync();
    }
    
    [MenuItem("Set product header")]
    public async Task SetProductHeader()
    {
        Settings.Current ??= Settings.Load();

        string header = Prompt.Show("Enter product header");
        Settings.Current.GitSettings.ProductHeader = header;
        Settings.Current.Save();
        await PrintSettingsAsync();
    }

    [MenuItem("Enter passphrase")]
    public async Task<string> EnterPassphrase()
    {
        _passPhrase = Prompt.ForPassword("Please enter passphrase");
        return _passPhrase;
    }
    
    [MenuItem("Save encrypted settings")]
    public async Task SaveEncryptedSettings()
    {
        string passPhrase = string.IsNullOrEmpty(_passPhrase) ? await EnterPassphrase() : _passPhrase;
        string filePath = SaveEncrypted(passPhrase);
        Settings.Current.DeleteUnencryptedSettings();
        Message.PrintLine();
        Message.PrintLine("Saved encrypted settings to {0}", filePath, ConsoleColor.Green);
    }

    [MenuItem("Load encrypted settings")]
    public async Task LoadEncryptedSettings()
    {
        string passPhrase = string.IsNullOrEmpty(_passPhrase) ? await EnterPassphrase() : _passPhrase;
        Settings.Current = LoadEncrypted(passPhrase) ?? Settings.Load();
        Message.PrintLine();
        await PrintSettingsAsync();
    }
    
    private static string _passPhrase = string.Empty;
    public static Settings? LoadEncrypted(string password)
    {
        string path = GetEncryptedFilePath();
        if (File.Exists(path))
        {
            string encrypted = File.ReadAllText(path);
            string yaml = Aes.Decrypt(encrypted, password);
            return yaml.FromYaml<Settings>(true);
        }

        return null;
    }
    
    public string SaveEncrypted(string passPhrase)
    {
        string yaml = Settings.Current.ToYaml();
        string encrypted = Aes.Encrypt(yaml, passPhrase);
        string path = GetEncryptedFilePath();
        File.WriteAllText(path, encrypted);
        return path;
    }
    
    public static string GetEncryptedFilePath()
    {
        return Path.Combine(Environment.CurrentDirectory, "settings.yaml.aes");
    }
}