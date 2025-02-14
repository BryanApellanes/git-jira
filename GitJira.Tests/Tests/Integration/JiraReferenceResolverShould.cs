using Bam;
using Bam.Console;
using Bam.CoreServices;
using Bam.Test;
using GitJira.Classes;
using GitJira.Interfaces;

namespace GitJira.tests.Tests.Integration;

public class JiraReferenceResolverShould : UnitTestMenuContainer
{
    public JiraReferenceResolverShould(ServiceRegistry serviceRegistry) : base(serviceRegistry)
    {
    }

    
    
    [UnitTest]
    public async Task ResolveJiraIdFromBody()
    {
        IJiraReferenceResolver jiraReferenceResolver = GitJiraServiceRegistry
            .GetRegistry()
            .Get<IJiraReferenceResolver>();

        jiraReferenceResolver.ContainsJiraId(@"
This is a fake comment
This is another line: OKTA-123456", out string jiraId).ShouldBeTrue("Failed to find jiraId");
        
        jiraId.ShouldBeEqualTo("OKTA-123456");
    }

    [UnitTest]
    public async Task GetAuthToken()
    {
        ProcessGitAuthTokenProvider processGitAuthTokenProvider = new ProcessGitAuthTokenProvider();
        string authToken = processGitAuthTokenProvider.GetGitAuthToken();
        Message.PrintLine(authToken);
    }
}