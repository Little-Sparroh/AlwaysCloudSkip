using System;
using System.IO;
using System.Threading;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;

[BepInPlugin(PluginGUID, PluginName, PluginVersion)]
[MycoMod(null, ModFlags.IsClientSide)]
public class SparrohPlugin : BaseUnityPlugin
{
    public const string PluginGUID = "sparroh.alwayscloudskip";
    public const string PluginName = "AlwaysCloudSkip";
    public const string PluginVersion = "1.0.0";

    internal static new ManualLogSource Logger;
    internal static ConfigEntry<bool> enableCloudSkip;

    private Harmony harmony;
    private FileSystemWatcher configWatcher;
    private DateTime lastReloadTime = DateTime.MinValue;
    private readonly object reloadLock = new object();

    private void Awake()
    {
        Logger = base.Logger;

        enableCloudSkip = Config.Bind(
            "General",
            "Enable Cloud Skip",
            true,
            "Enables Cloud Skip (double jump) at all times.");

        enableCloudSkip.SettingChanged += (_, __) =>
        {
            Logger.LogInfo($"Enable Cloud Skip set to {enableCloudSkip.Value}");
        };

        try
        {
            SetupFileWatcher();
        }
        catch (Exception ex)
        {
            Logger.LogError($"Error setting up file watcher: {ex.Message}");
        }

        harmony = new Harmony(PluginGUID);
        harmony.PatchAll(typeof(AlwaysCloudSkipPatches));

        Logger.LogInfo($"{PluginName} v{PluginVersion} loaded successfully.");
    }

    private void SetupFileWatcher()
    {
        configWatcher = new FileSystemWatcher(Paths.ConfigPath, $"{PluginGUID}.cfg")
        {
            NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size | NotifyFilters.FileName,
            EnableRaisingEvents = true
        };

        configWatcher.Changed += OnConfigFileChanged;
        configWatcher.Created += OnConfigFileChanged;
        configWatcher.Renamed += OnConfigFileChanged;
    }

    private void OnConfigFileChanged(object sender, FileSystemEventArgs e)
    {
        // Debounce: editors often fire multiple change events, and the file may still be locked.
        lock (reloadLock)
        {
            if ((DateTime.UtcNow - lastReloadTime).TotalMilliseconds < 250)
                return;
            lastReloadTime = DateTime.UtcNow;
        }

        ThreadPool.QueueUserWorkItem(_ =>
        {
            for (int attempt = 0; attempt < 5; attempt++)
            {
                try
                {
                    Thread.Sleep(50 * (attempt + 1));
                    Config.Reload();
                    Logger.LogInfo($"Config reloaded. Enable Cloud Skip = {enableCloudSkip.Value}");
                    return;
                }
                catch (IOException)
                {
                    // File still locked by editor; retry.
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Error reloading config: {ex.Message}");
                    return;
                }
            }

            Logger.LogWarning("Failed to reload config after several attempts (file may still be locked).");
        });
    }

    private void OnDestroy()
    {
        if (configWatcher != null)
        {
            configWatcher.EnableRaisingEvents = false;
            configWatcher.Changed -= OnConfigFileChanged;
            configWatcher.Created -= OnConfigFileChanged;
            configWatcher.Renamed -= OnConfigFileChanged;
            configWatcher.Dispose();
            configWatcher = null;
        }

        harmony?.UnpatchSelf();
    }
}
