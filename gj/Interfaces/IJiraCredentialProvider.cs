using GitJira;

namespace GitJira.Interfaces;

public interface IJiraCredentialProvider
{
    IJiraCredentials GetCredentials();
}