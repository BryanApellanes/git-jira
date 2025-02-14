using Bam.CommandLine;
using Bam.Console;
using GitJira.Interfaces;

namespace GitJira.Classes;

public class ProcessGitAuthTokenProvider : IGitAuthTokenProvider
{
    private readonly string _defaultAuthTokenCommand = $"/Library/Frameworks/Python.framework/Versions/3.11/bin/aurm auth github --scope okta";
    public ProcessGitAuthTokenProvider()
    {
        AuthTokenCommand = Settings.Current.GitSettings.AuthTokenCommand ?? _defaultAuthTokenCommand;
    }
    
    public string AuthTokenCommand { get; set; }
    public string GetGitAuthToken()
    {
        ProcessOutput output = Bam.Console.Extensions.Run(AuthTokenCommand);
        return output.StandardOutput.Trim();
    }
}