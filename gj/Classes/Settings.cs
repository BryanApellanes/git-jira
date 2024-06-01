using Bam;
using Bam;
using Bam.Encryption;
using GitJira.Interfaces;

namespace GitJira.Classes;

public class Settings
{
    static Settings()
    {
        Current = Load();
    }
    
    public Settings()
    {
    }
    
    public Settings(JiraSettings jiraSettings, GitSettings gitSettings)
    {
        this.JiraSettings = jiraSettings;
        this.GitSettings = gitSettings;
    }
    internal static Settings Current;
    public JiraSettings JiraSettings { get; set; }
    public GitSettings GitSettings { get; set; }

    public HashSet<string> Repliers { get; set; }
    public static Settings Load()
    {
        Settings? settings = LoadEncrypted();
        if (settings != null)
        {
            return settings;
        }
        
        string currentDirectory = Environment.CurrentDirectory;
        string settingsFile = Path.Combine(currentDirectory, "settings.yaml");
        if (File.Exists(settingsFile))
        {
            string yaml = File.ReadAllText(settingsFile);
            return yaml.FromYaml<Settings>();
        }

        throw new InvalidOperationException($"settings.yaml file not found: {settingsFile}");
    }

    public static Settings? LoadEncrypted()
    {
        string path = GetEncryptedFilePath();
        if (File.Exists(path))
        {
            string encrypted = File.ReadAllText(path);
            string yaml = Aes.Decrypt(encrypted);
            return yaml.FromYaml<Settings>(true);
        }

        return null;
    }
    
    public void SaveEncrypted()
    {
        string yaml = this.ToYaml();
        string encrypted = Bam.Encryption.Aes.Encrypt(yaml);
        string path = GetEncryptedFilePath();
        File.WriteAllText(path, encrypted);
    }

    public static string GetEncryptedFilePath()
    {
        return Path.Combine(Environment.CurrentDirectory, "settings.yaml.aes");
    }
}