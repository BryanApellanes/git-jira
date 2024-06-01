using Bam;
using GitJira.Interfaces;

namespace GitJira.Classes;

public class FileGitAuthTokenProvider : IGitAuthTokenProvider
{
    public FileGitAuthTokenProvider(string filePath) : this(new FileInfo(filePath))
    {
    }

    public FileGitAuthTokenProvider(FileInfo file)
    {
        this.File = file;
    }
    
    public FileInfo File { get; set; }
    
    public string GetGitAuthToken()
    {
        if (File.Exists)
        {
            return System.IO.File.ReadAllText(File.FullName);
        }

        return string.Empty;
    }
}