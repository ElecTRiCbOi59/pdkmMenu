using BepInEx.Configuration;
using UnityEngine;

public abstract class ConfigCategory
{
    protected ConfigFile Config;

    protected ConfigCategory(ConfigFile config)
    {
        Config = config;
        BindConfigs();
    }

    protected abstract void BindConfigs();

    protected ConfigEntry<T> Bind<T>(string section, string key, T defaultValue, string description = "")
    {
        return Config.Bind(section, key, defaultValue, description);
    }
    public void ToggleConfigEntry(ConfigEntry<bool> configEntry)
    {
        configEntry.Value = !configEntry.Value;
    }
    public virtual void CheckKeys()
    {
        return;
    }
}
