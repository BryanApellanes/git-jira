namespace GitJira.Interfaces;

public interface IOutput
{
    void Info(string message);
    void Warn(string message);
    void Error(string message);
    void Error(Exception ex);
    void Error(string message, Exception ex);
}