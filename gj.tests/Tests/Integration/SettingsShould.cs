using Bam;
using Bam.Console;
using Bam.CoreServices;
using Bam.Testing;
using GitJira.Classes;
using GitJira.Interfaces;

namespace GitJira.tests.Tests.Integration;

public class SettingsShould : UnitTestMenuContainer
{
    public SettingsShould(ServiceRegistry serviceRegistry) : base(serviceRegistry)
    {
    }

    [UnitTest]
    public async Task SaveAndLoadEncrypted()
    {
        ISettingsProvider settingsProvider = GitJiraServiceRegistry
            .GetRegistry()
            .Get<ISettingsProvider>();

        Settings settings = settingsProvider.GetSettings();
        settings.SaveEncrypted();
        string encrypted = await File.ReadAllTextAsync(Settings.GetEncryptedFilePath());
        
        Message.PrintLine(encrypted);

        Settings loaded = Settings.Load();
        loaded.JiraSettings.Url.ShouldEqual(settings.JiraSettings.Url);
        Message.PrintLine(loaded.JiraSettings.Url);
    }
}