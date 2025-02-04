using GitJira.Classes;

namespace GitJira.Interfaces;

public interface ISettingsProvider
{
    Settings GetSettings();
}