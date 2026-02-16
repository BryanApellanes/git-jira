# GitJira

A console application for managing the bidirectional workflow between GitHub Issues and Jira, with support for creating, commenting, closing, and syncing issues across both platforms.

## Overview

GitJira is an interactive console tool that bridges GitHub Issues and Atlassian Jira. It uses the BAM console menu system (`BamConsoleContext.StaticMain`) to provide a menu-driven interface for common cross-platform issue management tasks: loading GitHub issues across multiple repositories, creating corresponding Jira tickets, posting comments to either platform, closing/resolving issues, and tracking reply status.

The core abstraction is `ICompositeIssue`, which pairs a GitHub `Issue` (via Octokit) with a Jira `Issue` (via Atlassian.SDK). The `CompositeIssueManager` orchestrates all operations: listing GitHub issues, resolving Jira references from GitHub issue bodies, creating Jira tickets from GitHub issues (with labels, descriptions, and component assignment), posting comments with user mentions, transitioning Jira workflow states, and closing GitHub issues. The `JiraReferenceResolver` extracts Jira IDs (e.g., "PROJ-12345") from issue text.

Settings are managed through YAML files with support for AES encryption of sensitive credentials (Jira tokens, GitHub auth tokens). The tool supports multiple GitHub repositories and can process auth tokens either from settings or via an external process command. The DI container is configured through `GitJiraServiceRegistry`.

## Key Classes

| Class | Description |
|---|---|
| `IssueManagementMenu` | Main interactive menu: load issues, select repo, create Jira, add comments, close issues |
| `SettingsMenu` | Settings menu: configure Jira/GitHub credentials, save/load encrypted settings |
| `CompositeIssue` | Pairs a GitHub Issue with a Jira Issue; tracks reply status and closure state |
| `CompositeIssueManager` | Core orchestrator for all cross-platform issue operations |
| `CompositeIssueLoader` | Loads and assembles CompositeIssue instances from GitHub and Jira |
| `JiraReferenceResolver` | Extracts Jira issue IDs from text using regex patterns |
| `JiraToGitHubCommentProvider` | Generates GitHub comments from Jira issue data |
| `GitHubClientProvider` | Creates authenticated Octokit GitHubClient instances |
| `JiraClientProvider` | Creates authenticated Atlassian Jira client instances |
| `Settings` | Root settings object with Git, Jira, and canned response configuration |
| `GitSettings` | GitHub configuration: owner, repos, auth token, product header |
| `JiraSettings` | Jira configuration: URL, project key, parent key, issue type, component |
| `JiraCredentials` | Jira username and password/token |
| `GitJiraServiceRegistry` | DI configuration registering all services |
| `GitHubRepoIdentifier` | Identifies a GitHub repository by owner and name |
| `GitHubIssueIdentifier` | Identifies a GitHub issue by owner, repo, and number |
| `CannedResponse` | Pre-configured response text for common replies |
| `YamlFileSettingsProvider` | Loads settings from YAML files |
| `ProcessGitAuthTokenProvider` | Gets GitHub auth token by executing an external process |
| `SettingsGitAuthTokenProvider` | Gets GitHub auth token from settings |

## Dependencies

**Project References:**
- `bam.base` -- Core BAM framework
- `bam.console` -- Console menu infrastructure
- `bam.data` -- Data access layer
- `bam.encryption` -- AES encryption for settings

**Package References:**
- `Atlassian.SDK` 13.0.0 -- Jira REST API client
- `Octokit` 14.0.0 -- GitHub REST API client

**Target Framework:** net10.0
**Output Type:** Exe
**PublishAot:** true

## Usage Examples

```bash
# Run interactively
dotnet run --project GitJira

# The menu system provides these options:
# Issue Management:
#   - (Re)Load issues
#   - Select repo
#   - Show issues for current repo
#   - Select Issue
#   - Show issue description / comments
#   - Create Jira and reply to GitHub issue
#   - Add GitHub comment / Add Jira comment
#   - Close GitHub issue with comment
#   - Close or resolve Jira with comment
#
# Settings:
#   - Print settings
#   - Set Jira/GitHub credentials
#   - Save/Load encrypted settings
```

```csharp
using GitJira;
using GitJira.Interfaces;

// Get the service registry
var registry = GitJiraServiceRegistry.GetRegistry();

// Use the composite issue manager
var manager = registry.Get<ICompositeIssueManager>();

// List GitHub issues
var issues = await manager.ListGitHubIssuesAsync("owner", "repo");

// Create a Jira from a composite issue
var jiraIssue = await manager.CreateJiraIssueAsync(compositeIssue, "Thanks for reporting!");

// Close a GitHub issue
var result = await manager.CloseGithubIssueWithCommentAsync(compositeIssue, "Fixed in latest release");
```

## Known Gaps / Not Yet Implemented

- No automated pagination for GitHub issue listing -- all issues are fetched in a single call.
- `GetCompositeIssues` and `HasReplyAsync` use `.Result` on async calls, which can cause deadlocks.
- The `CompositeIssue.ReplyStatus` getter performs synchronous `.Result` calls on async methods.
- No error handling/retry logic for API rate limiting from GitHub or Jira.
- The `PublishAot` flag is enabled but AOT compatibility with Atlassian.SDK and Octokit has not been verified.
- The `settings.yaml` file is included but contains no default values.
