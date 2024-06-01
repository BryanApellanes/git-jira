using GitJira.Interfaces;

namespace GitJira.Classes;

public class GitHubRepoIdentifier
{
    public string Owner { get; set; }
    public string RepositoryName { get; set; }

    public long GetRepoId(ICompositeClient client)
    {
        return client.GetRepository(this).Id;
    }
}