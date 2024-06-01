using Bam.Console;
using Bam.Shell;
using GitJira.Classes;

namespace Bam.Application;

[Menu("Options")]
public class MainMenu: MenuContainer
{
    private static Settings Current;
    [MenuItem("Print settings")]
    public async Task PrintSettings()
    {
        Current ??= Settings.Load();
        Message.Print(Current.ToYaml());
    }
}