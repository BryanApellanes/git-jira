using Bam;
using Bam.CoreServices;
using Bam.Logging;
using GitJira.Classes;
using GitJira.Interfaces;

namespace GitJira;

public class GitJiraServiceRegistry
{
    public static ServiceRegistry GetRegistry(Func<ServiceRegistry, ServiceRegistry> configure = null)
    {
        configure ??= (svcReg) => svcReg;

        return configure(new ServiceRegistry()
            .For<IGitAuthTokenProvider>().Use(new FileGitAuthTokenProvider("D:/gh.txt"))
            .For<IJiraSettingsProvider>().Use<JiraSettingsProvider>()
            .For<IGitSettingsProvider>().Use<GitSettingsProvider>()
            .For<IGitHubClientProvider>().Use<GitHubClientProvider>()
            .For<ICompositeIssueResolver>().Use<CompositeIssueResolver>()
            .For<IJiraClientProvider>().Use<JiraClientProvider>()
            .For<ICompositeClient>().Use<CompositeClient>()
            .For<IApplicationNameProvider>().Use<ProcessApplicationNameProvider>()
            .For<ISettingsProvider>().Use<YamlFileSettingsProvider>()
            .For<ILogger>().Use<TextFileLogger>());
    }
}