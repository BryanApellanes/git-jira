using Bam;
using Bam.Console;
using Bam.DependencyInjection;
using Bam.Logging;
using Bam.Services;
using GitJira.Classes;
using GitJira.Interfaces;

namespace GitJira;

public class GitJiraServiceRegistry
{
    public static ServiceRegistry GetRegistry(Func<ServiceRegistry, ServiceRegistry> configure = null)
    {
        configure ??= (svcReg) => svcReg;

        ServiceRegistry svcRegistry = configure(new ServiceRegistry()
            .For<IJiraSettingsProvider>().Use<JiraSettingsProvider>());

        if (!string.IsNullOrEmpty(Settings.Current.GitSettings.AuthTokenCommand))
        {
            svcRegistry.For<IGitAuthTokenProvider>().Use<ProcessGitAuthTokenProvider>();
        }
        else
        {
            svcRegistry.For<IGitAuthTokenProvider>().Use(new SettingsGitAuthTokenProvider());
        }
        
        svcRegistry
            .For<IGitSettingsProvider>().Use<GitSettingsProvider>();
        
        svcRegistry
            .For<IGitHubClientProvider>().Use(svcRegistry.Get<GitHubClientProvider>())
            .For<ICompositeIssueLoader>().Use<CompositeIssueLoader>()
            .For<ISettingsProvider>().Use<YamlFileSettingsProvider>()
            .For<IJiraClientProvider>().Use<JiraClientProvider>()
            .For<IApplicationNameProvider>().Use<ProcessApplicationNameProvider>()
            .For<IJiraReferenceResolver>().Use<JiraReferenceResolver>()
            .For<IJiraToGitHubCommentProvider>().Use<JiraToGitHubCommentProvider>()
            .For<ILogger>().Use<ConsoleLogger>()
            .For<ICompositeIssueManager>().Use<CompositeIssueManager>();

        return svcRegistry;
    }
}