using System.Runtime.Intrinsics.X86;
using Amazon.SecurityToken.Model;
using Bam;
using GitJira.Interfaces;

namespace GitJira.Classes;

public class GitSettings : IGitSettings
{
    public string OwnerName { get; set; } 
    public string UserName { get; set; }
    public string DefaultRepositoryName { get; set; }
    public string[] RepositoryNames { get; set; }
    public string ProductHeader { get; set;}
    public string AuthToken { get; set; }
    
    public string AuthTokenCommand { get; set; }
    public string AuthTokenScope { get; set; }

    public IEnumerable<GitHubRepoIdentifier> GetOrgRepoIdentifiers()
    {
        foreach (string repoName in RepositoryNames)
        {
            yield return new GitHubRepoIdentifier()
            {
                Owner = OwnerName,
                RepositoryName = repoName
            };
        }
    }
    public GitHubRepoIdentifier GetOrgRepoIdentifier()
    {
        return new GitHubRepoIdentifier()
        {
            Owner = OwnerName,
            RepositoryName = DefaultRepositoryName
        };
    }

    public GitHubRepoIdentifier GetUserRepoIdentifier()
    {
        return new GitHubRepoIdentifier()
        {
            Owner = UserName,
            RepositoryName = DefaultRepositoryName
        };
    }
}