namespace GitJira;

public interface IJiraCredentials
{
    string UserName { get; }
    string Password { get; }
}