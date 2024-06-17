using Bam.CommandLine;
using GitJira.Interfaces;

namespace GitJira.Classes;

public class ProcessGitAuthTokenProvider : IGitAuthTokenProvider
{
    public string GetGitAuthToken()
    {
        ProcessOutput output = "/Library/Frameworks/Python.framework/Versions/3.11/bin/aurm auth github --scope okta".Run();
        return output.StandardOutput;
    }
}