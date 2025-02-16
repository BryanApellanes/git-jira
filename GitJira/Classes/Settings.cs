using Bam;
using Bam.Encryption;

namespace GitJira.Classes;

public class Settings
{
    private const string Password = "GoodEnoughForNow";
        
    static Settings()
    {
        Current = Load();
    }
    
    public Settings()
    {
        this.Repliers = new HashSet<string>();
        this.Repliers.Append("bryan.apellanes@okta.com");
    }
    
    public Settings(JiraSettings jiraSettings, GitSettings gitSettings)
    {
        this.JiraSettings = jiraSettings;
        this.GitSettings = gitSettings;
    }
    
    internal static Settings Current;
    public JiraSettings JiraSettings { get; set; }
    public GitSettings GitSettings { get; set; }

    /// <summary>
    /// Gets or sets the GitHub usernames used to determine if a reply has been made on the issue.
    /// </summary>
    public HashSet<string> Repliers { get; set; }
    
    public List<string> CannedResponses { get; set; }
    
    private static Settings _settings;
    public static Settings Load()
    {
        if (_settings == null)
        {
            string currentDirectory = Environment.CurrentDirectory;
            string settingsFile = Path.Combine(currentDirectory, "settings.yaml");
            if (File.Exists(settingsFile))
            {
                string yaml = File.ReadAllText(settingsFile);
                _settings = yaml.FromYaml<Settings>();
            }
            else
            {
                _settings = new Settings();
                string yaml = _settings.ToYaml();
                File.WriteAllText(settingsFile, yaml);
            }
        }

        return _settings;
    }

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
    
    public void Save()
    {
        string yaml = this.ToYaml();
        string path = Path.Combine(Environment.CurrentDirectory, "settings.yaml");
        File.WriteAllText(path, yaml);
    }

    public void DeleteUnencryptedSettings()
    {
        string path = Path.Combine(Environment.CurrentDirectory, "settings.yaml");
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }
    
    public static string GetEncryptedFilePath()
    {
        return Path.Combine(Environment.CurrentDirectory, "settings.yaml.aes");
    }
}