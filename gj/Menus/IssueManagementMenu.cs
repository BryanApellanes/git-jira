using Bam;
using Bam.Console;
using Bam.Shell;
using GitJira.Classes;
using GitJira.Interfaces;
using Octokit;

namespace GitJira.Menus;

[Menu("Issue Management")]
public class IssueManagementMenu : MenuContainer
{
    [MenuItem("List Git Issues")]
    public async Task ShowIssue()
    {
        Settings.Current ??= Settings.Load();
        Message.Print(Settings.Current.ToYaml());
        ICompositeClient client = GitJiraServiceRegistry.GetRegistry().Get<ICompositeClient>();
        List<Repository> repositories = await client.ListRepositoriesForUser();
        foreach (Repository repo in repositories)
        {
            Message.PrintLine(repo.Name);
            Message.PrintLine(repo.Id.ToString());
        }
    }
}