using System;
using System.IO;
using BepInEx;
using TTMod.Core.Plugins;
using UnityEngine;

namespace TTMod.SaveGameManager.Services;

/// <summary>
/// Soft compatibility with Endless Campaign: reads loop counts from its profile JSON
/// without referencing TTMod.EndlessCampaign.dll.
/// </summary>
internal static class EndlessCampaignCompat
{
    internal const string PluginGuid = "com.tabletop-tavern.endless-campaign";

    private const string ProfileFileName = "com.tabletop-tavern.endless-campaign-profile.json";

    private static bool _profileLoaded;
    private static EndlessCampaignProfileSnapshot _profile = new();

    internal static bool IsModLoaded => TtPluginRegistry.IsLoaded(PluginGuid);

    internal static bool TryGetLoopCount(string runId, out int loopCount)
    {
        loopCount = 0;

        if (!IsModLoaded || string.IsNullOrWhiteSpace(runId))
        {
            return false;
        }

        EnsureProfileLoaded();

        foreach (var activeRun in _profile.ActiveRuns)
        {
            if (string.Equals(activeRun.RunUuid, runId, StringComparison.Ordinal))
            {
                loopCount = activeRun.LoopCount;
                return true;
            }
        }

        // Endless Campaign is loaded but this run is not in the profile yet.
        return true;
    }

    internal static void InvalidateCache()
    {
        _profileLoaded = false;
        _profile = new EndlessCampaignProfileSnapshot();
    }

    private static void EnsureProfileLoaded()
    {
        if (_profileLoaded)
        {
            return;
        }

        _profileLoaded = true;
        _profile = new EndlessCampaignProfileSnapshot();

        var path = Path.Combine(Paths.ConfigPath, ProfileFileName);
        if (!File.Exists(path))
        {
            return;
        }

        try
        {
            var json = File.ReadAllText(path);
            _profile = JsonUtility.FromJson<EndlessCampaignProfileSnapshot>(json)
                ?? new EndlessCampaignProfileSnapshot();
            _profile.ActiveRuns ??= Array.Empty<EndlessCampaignActiveRunSnapshot>();
        }
        catch (Exception ex)
        {
            Plugin.LogSource.LogWarning($"Endless Campaign profile read failed: {ex.Message}");
            _profile = new EndlessCampaignProfileSnapshot();
        }
    }

    [Serializable]
    private sealed class EndlessCampaignProfileSnapshot
    {
        public EndlessCampaignActiveRunSnapshot[] ActiveRuns = Array.Empty<EndlessCampaignActiveRunSnapshot>();
    }

    [Serializable]
    private sealed class EndlessCampaignActiveRunSnapshot
    {
        public string RunUuid = string.Empty;
        public int LoopCount;
    }
}
