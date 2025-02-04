using GitJira.Interfaces;

namespace GitJira.Classes;

public class GitHubRepoIdentifier
{
    public string Owner { get; set; }
    public string RepositoryName { get; set; }

    public override bool Equals(object? obj)
    {
        if (obj is GitHubRepoIdentifier identifier)
        {
            return identifier.Owner.Equals(Owner) && identifier.RepositoryName.Equals(RepositoryName); 
        }

        return false;
    }

    public override int GetHashCode()
    {
        return Owner.GetHashCode() * 17 + RepositoryName.GetHashCode();
    }
}