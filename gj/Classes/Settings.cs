using Bam;
using Bam;
using Bam.Encryption;
using GitJira.Interfaces;

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

    public HashSet<string> Repliers { get; set; }
    
    public List<string> CannedResponses { get; set; }
    
    private static Settings _settings;
    public static Settings Load()
    {
        // TODO: Handle encryption
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
                throw new InvalidOperationException($"settings.yaml file not found: {settingsFile}");
            }
        }

        return _settings;
    }

    public static Settings? LoadEncrypted()
    {
        string path = GetEncryptedFilePath();
        if (File.Exists(path))
        {
            string encrypted = File.ReadAllText(path);
            string yaml = Aes.Decrypt(encrypted, Password);
            return yaml.FromYaml<Settings>(true);
        }

        return null;
    }
    
    public void Save()
    {
        string yaml = this.ToYaml();
        //string encrypted = Bam.Encryption.Aes.Encrypt(yaml, Password);
        string path = Path.Combine(Environment.CurrentDirectory, "settings.yaml");
        File.WriteAllText(path, yaml);
    }

    public static string GetEncryptedFilePath()
    {
        return Path.Combine(Environment.CurrentDirectory, "settings.yaml.aes");
    }
}