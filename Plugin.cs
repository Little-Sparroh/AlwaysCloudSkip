using System;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

[BepInPlugin(PluginGUID, PluginName, PluginVersion)]
[MycoMod(null, ModFlags.IsSandbox)]
public class SparrohPlugin : BaseUnityPlugin
{
    public const string PluginGUID = "sparroh.alwayscloudskip";
    public const string PluginName = "AlwaysCloudSkip";
    public const string PluginVersion = "1.0.1";

    internal new static ManualLogSource Logger;

    private Harmony harmony;

    private void Awake()
    {
        Logger = base.Logger;

        ConfigManager.Initialize(Config, Logger);

        harmony = new Harmony(PluginGUID);

        try
        {
            harmony.PatchAll(typeof(AlwaysCloudSkipPatches));
            Logger.LogInfo("Harmony patches applied.");
        }
        catch (Exception ex)
        {
            Logger.LogError($"Error applying patches: {ex.Message}");
        }

        Logger.LogInfo($"{PluginName} v{PluginVersion} loaded successfully.");
    }

    private void Update()
    {
        ConfigManager.Tick();
    }

    private void OnDestroy()
    {
        ConfigManager.Dispose();
        harmony?.UnpatchSelf();
    }
}