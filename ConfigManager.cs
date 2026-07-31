using System;
using System.IO;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using UnityEngine;

public static class ConfigManager
{
    private const float DebounceSeconds = 0.25f;

    private static ConfigFile config;
    private static ManualLogSource logger;
    private static FileSystemWatcher configWatcher;
    private static volatile bool reloadPending;
    private static float lastReloadTime;
    public static ConfigEntry<bool> EnableCloudSkip { get; private set; }

    public static void Initialize(ConfigFile configFile, ManualLogSource log)
    {
        config = configFile;
        logger = log;

        EnableCloudSkip = config.Bind(
            "General",
            "Enable Cloud Skip",
            true,
            "Enables Cloud Skip (double jump) at all times.");

        EnableCloudSkip.SettingChanged += OnSettingChanged;

        try
        {
            SetupFileWatcher();
        }
        catch (Exception ex)
        {
            logger.LogError($"Error setting up config file watcher: {ex.Message}");
        }
    }


    public static void Tick()
    {
        if (!reloadPending)
            return;

        if (Time.unscaledTime - lastReloadTime < DebounceSeconds)
            return;

        reloadPending = false;
        lastReloadTime = Time.unscaledTime;

        try
        {
            config.Reload();
            logger.LogInfo($"Config reloaded. Enable Cloud Skip = {EnableCloudSkip.Value}");
        }
        catch (Exception ex)
        {
            logger.LogError($"Error reloading config: {ex.Message}");
        }
    }

    public static void Dispose()
    {
        if (EnableCloudSkip != null)
            EnableCloudSkip.SettingChanged -= OnSettingChanged;

        if (configWatcher != null)
        {
            configWatcher.EnableRaisingEvents = false;
            configWatcher.Changed -= OnConfigFileChanged;
            configWatcher.Created -= OnConfigFileChanged;
            configWatcher.Renamed -= OnConfigFileChanged;
            configWatcher.Dispose();
            configWatcher = null;
        }
    }

    private static void SetupFileWatcher()
    {
        configWatcher = new FileSystemWatcher(Paths.ConfigPath, $"{SparrohPlugin.PluginGUID}.cfg");
        configWatcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size | NotifyFilters.FileName;
        configWatcher.Changed += OnConfigFileChanged;
        configWatcher.Created += OnConfigFileChanged;
        configWatcher.Renamed += OnConfigFileChanged;
        configWatcher.EnableRaisingEvents = true;
    }

    private static void OnConfigFileChanged(object sender, FileSystemEventArgs e)
    {
        reloadPending = true;
    }

    private static void OnSettingChanged(object sender, EventArgs e)
    {
        logger?.LogInfo($"Enable Cloud Skip set to {EnableCloudSkip.Value}");
    }
}