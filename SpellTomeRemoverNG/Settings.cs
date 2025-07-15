using Mutagen.Bethesda.WPF.Reflection.Attributes;

namespace SpellTomeRemoverNG
{
    
    public class Settings
    {
        [SettingName("Plugin Blacklist")]
        public List<string> PluginBlackList = [];
    }
}
