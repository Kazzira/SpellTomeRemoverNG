using Mutagen.Bethesda.WPF.Reflection.Attributes;

namespace SpellTomeRemoverNG;

/// <summary>
/// Represents an entry in the plugin blacklist.
/// This record contains information about a plugin that should be skipped during the patching process.
/// </summary>
/// <param name="PluginFileName">The file name of the plugin to be blacklisted.</param>
/// <param name="SkipLeveledLists">Indicates whether leveled lists from this plugin should be skipped.</param>
/// <param name="SkipContainers">Indicates whether containers from this plugin should be skipped.</param>
/// <param name="SkipPlacedObjects">Indicates whether placed objects from this plugin should be skipped.</param>
/// <param name="SkipBooks">Indicates whether books from this plugin should be skipped.</param>
/// <remarks>
/// This record is used to define which plugins should be excluded from the patching process,
/// allowing users to customize the behavior of the Spell Tome Remover.
/// </remarks>
public record PluginBlacklistEntry(
    string PluginFileName,
    bool SkipLeveledLists,
    bool SkipContainers,
    bool SkipPlacedObjects,
    bool SkipBooks
);


/// <summary>
/// Represents the settings for the Spell Tome Remover.
/// </summary>
/// <remarks>
/// This class contains the configuration settings for the Spell Tome Remover,
/// including a blacklist of plugins that should be skipped during the patching process.
/// </remarks>
public class Settings
{
    /// <summary>
    /// Singleton instance of the Settings class.
    /// </summary>
    public static Lazy<Settings> Instance = null!;

    /// <summary>
    /// Gets or sets the plugin blacklist.
    /// </summary>
    [SettingName("Plugin Blacklist")]
    public List<PluginBlacklistEntry> PluginBlackList = [];

    /// <summary>
    /// Gets or sets a value indicating whether to run the patcher in quiet mode.
    /// If true, the patcher will not output messages to the console.
    /// </summary>
    [SettingName("Quiet Run")]
    public bool QuietRun { get; set; } = false;
}