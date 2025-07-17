using Mutagen.Bethesda.WPF.Reflection.Attributes;

namespace SpellTomeRemoverNG
{
    public record PluginBlacklistEntry(string PluginFileName, bool BlacklistLeveledLists, bool BlacklistContainers, bool BlacklistPlacedObjects, bool BlacklistBooks);
    public class Settings
    {
        [SettingName("Plugin Blacklist")]
        public List<PluginBlacklistEntry> PluginBlackList = [];
    }
}
