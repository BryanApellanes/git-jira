# GitJira.Tests

Integration and unit tests for the GitJira application, validating Jira reference resolution and GitHub authentication.

## Overview

GitJira.Tests is a console-based test runner for the GitJira project. It uses the BAM test framework (`BamConsoleContext.StaticMain` with `[UnitTest]` attributes and `UnitTestMenuContainer`) rather than xUnit or NUnit. The test project validates core functionality of the GitJira system, particularly the ability to extract Jira issue IDs from text and to retrieve GitHub authentication tokens.

The project includes a `MainMenu` class that provides a settings inspection option, allowing testers to verify the current YAML configuration before running tests. The `JiraReferenceResolverShould` test class contains integration tests that exercise the `JiraReferenceResolver` and `ProcessGitAuthTokenProvider` components.

## Key Classes

| Class | Description |
|---|---|
| `Program` | Entry point using `BamConsoleContext.StaticMain` |
| `MainMenu` | Menu container with a "Print settings" option for configuration inspection |
| `JiraReferenceResolverShould` | Integration test class with Jira reference resolution and auth token tests |

## Test Methods

| Test | Description |
|---|---|
| `ResolveJiraIdFromBody` | Verifies that `JiraReferenceResolver.ContainsJiraId` correctly extracts a Jira ID (e.g., "OKTA-123456") from multi-line text |
| `GetAuthToken` | Integration test that exercises `ProcessGitAuthTokenProvider` to retrieve a GitHub auth token via an external process |

## Dependencies

**Project References:**
- `bam.test` -- BAM test framework (`UnitTestMenuContainer`, `[UnitTest]` attribute)
- `GitJira` -- The project under test

**Target Framework:** net10.0
**Output Type:** Exe

## Usage Examples

```bash
# Run all unit tests
dotnet run --project GitJira.Tests -- --ut

# Run interactively to access the menu
dotnet run --project GitJira.Tests
```

## Known Gaps / Not Yet Implemented

- Only two test methods exist; there is no coverage for `CompositeIssueManager`, `CompositeIssueLoader`, `IssueManagementMenu`, or `SettingsMenu`.
- The `GetAuthToken` test depends on an external process being configured in settings, making it environment-dependent.
- No mock-based unit tests -- the `ResolveJiraIdFromBody` test instantiates the full service registry.
- No tests for Jira issue creation, GitHub comment posting, issue closure, or workflow transitions.
- No tests for encrypted settings save/load.
