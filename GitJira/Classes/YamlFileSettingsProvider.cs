using Bam.Logging;
using GitJira.Interfaces;

namespace GitJira.Classes;

public class YamlFileSettingsProvider : ISettingsProvider
{
    public YamlFileSettingsProvider(ILogger logger)
    {
        this.Logger = logger;
    }
    
    protected ILogger Logger { get; init; }
    
    public Settings GetSettings()
    {
        return Settings.Current ?? Settings.Load();
    }
}