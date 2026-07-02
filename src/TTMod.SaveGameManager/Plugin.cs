using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using TTMod.Core;
using TTMod.Core.Config;
using TTMod.Core.Patching;
using TTMod.SaveGameManager.Runtime;
using UnityEngine;

namespace TTMod.SaveGameManager;

[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
[BepInDependency(TtCore.PluginGuid, BepInDependency.DependencyFlags.HardDependency)]
public sealed class Plugin : BaseUnityPlugin
{
    public const string PluginGuid = "com.tabletop-tavern.save-game-manager";
    public const string PluginName = "Save Game Manager";
    public const string PluginVersion = "0.1.1";

    internal static ManualLogSource LogSource = null!;

    private Harmony? _harmony;

    private void Awake()
    {
        LogSource = Logger;

        SaveGameManagerSettings.Initialize(Config);

        _harmony = TtHarmony.PatchAssembly(PluginGuid, typeof(Plugin).Assembly, Logger);

        var bootstrap = new GameObject("TabletopTavernSaveGameManagerBootstrap");
        bootstrap.hideFlags = HideFlags.HideAndDontSave;
        bootstrap.AddComponent<SaveGameManagerBootstrap>();

        LogSource.LogInfo($"{PluginName} {PluginVersion} loaded.");
    }

    private void OnDestroy()
    {
        _harmony?.UnpatchSelf();
    }
}
