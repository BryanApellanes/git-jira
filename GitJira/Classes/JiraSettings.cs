using Bam;

namespace GitJira.Classes;

public class JiraSettings : IJiraSettings
{
    public const string DefaultProjectKey = "OKTA";
    public const string DefaultIssueType = "Internal Story";
    public const string DefaultComponent = "Team: Developer Community Products";
    public const string DefaultParentKey = "OKTA-728925";
    
    private string urlFormat = "https://{0}.atlassian.net/";
    public JiraSettings()
    {
        Url = string.Format(urlFormat, "oktainc");
        ProjectKey = DefaultProjectKey;
        IssueType = DefaultIssueType;
        Component = DefaultComponent;
    }
    
    public string Url { get; set; }
    public JiraCredentials Credentials { get; set; }
    public string ProjectKey { get; set; }
    public string IssueType { get; set; }
    public string Component { get; set; }

    public string ParentKey { get; set; }

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