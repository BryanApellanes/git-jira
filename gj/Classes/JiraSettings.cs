using Bam;
using GitJira;

namespace GitJira.Classes;

public class JiraSettings : IJiraSettings
{
    private string urlFormat = "https://{0}.atlassian.net/";
    public JiraSettings()
    {
        this.Url = string.Format(urlFormat, "www");
    }

    public JiraSettings(string subdomain)
    {
        this.Url = string.Format(urlFormat, subdomain);
    }
    
    public JiraSettings(string subdomain, string username, string password)
    {
        this.Url = string.Format(urlFormat, subdomain);
        this.Credentials = new JiraCredentials
        {
            UserName = username,
            Password = password
        };
    }
    
    public string Url { get; set; }
    public JiraCredentials Credentials { get; set; }

    private static JiraSettings _default;
    private static object _defaultLock;
    public static JiraSettings Default
    {
        get
        {
            return _defaultLock.DoubleCheckLock<JiraSettings>(ref _default, () => new JiraSettings());
        }
        set => _default = value;
    }
}