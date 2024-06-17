using System.Runtime.Intrinsics.X86;
using Amazon.SecurityToken.Model;
using Bam;
using GitJira.Interfaces;

namespace GitJira.Classes;

public class GitSettings : IGitSettings
{
    public string OwnerName { get; set; } 
    public string UserName { get; set; }
    public string RepositoryName { get; set; }
    public string ProductHeader { get; set;}
    public string AuthToken { get; set; }

    public GitHubRepoIdentifier GetOrgRepoIdentifier()
    {
        return new GitHubRepoIdentifier()
        {
            Owner = OwnerName,
            RepositoryName = RepositoryName
        };
    }

    public GitHubRepoIdentifier GetUserRepoIdentifier()
    {
        return new GitHubRepoIdentifier()
        {
            Owner = UserName,
            RepositoryName = RepositoryName
        };
    }
}