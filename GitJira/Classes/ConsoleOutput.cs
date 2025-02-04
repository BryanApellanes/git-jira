using Bam;
using Bam.Console;
using GitJira.Interfaces;

namespace GitJira.Classes;

public class ConsoleOutput : IOutput
{
    public void Info(string message)
    {
        Message.PrintLine(message, ConsoleColor.Cyan);
    }

    public void Warn(string message)
    {
        Message.PrintLine(message, ConsoleColor.Yellow);
    }

    public void Error(string message)
    {
        Message.PrintLine(message, ConsoleColor.Red);
    }

    public void Error(Exception ex)
    {
        Message.PrintLine(ex.Message, ConsoleColor.Red);
        Message.PrintLine(ex.GetStackTrace(), ConsoleColor.Red);
    }

    public void Error(string message, Exception ex)
    {
        string msg = message;
        if (!ex.Message.Equals(msg))
        {
            msg += $"\r\n{ex.Message}";
        }
        Message.PrintLine(msg, ConsoleColor.Red);
        Message.PrintLine(ex.GetStackTrace(), ConsoleColor.Red);
    }
}