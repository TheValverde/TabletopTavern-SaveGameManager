using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Memori.SaveData;
using TTMod.SaveGameManager.Models;
using UnityEngine;

namespace TTMod.SaveGameManager.Services;

internal static class RunSlotStore
{
    private const string ManifestFileName = "manifest.json";

    private static readonly object Sync = new();
    private static bool _loaded;
    private static RunSlotManifest _manifest = new();

    internal static void EnsureLoaded()
    {
        lock (Sync)
        {
            if (_loaded)
            {
                return;
            }

            LoadManifest();
            _loaded = true;
        }
    }

    internal static IReadOnlyList<RunListEntry> GetParkedRuns()
    {
        EnsureLoaded();
        return _manifest.Runs
            .Select(metadata => metadata.ToEntry())
            .ToList();
    }

    internal static void ParkActiveRun()
    {
        if (!SaveDataHandler.CampaignSaveExists())
        {
            return;
        }

        var save = SaveDataHandler.Load();
        if (save == null || save.blank)
        {
            return;
        }

        ParkSave(save);
    }

    internal static void ParkSave(CampaignSaveData save)
    {
        EnsureLoaded();

        var runId = RunCatalogService.ResolveRunId(save);
        var json = JsonUtility.ToJson(save);
        var path = GetParkedSavePath(runId);
        Directory.CreateDirectory(GetRunsDirectory());
        File.WriteAllText(path, json);

        var metadata = RunSlotMetadata.FromSave(save, runId, DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture));
        _manifest.Runs.RemoveAll(entry => entry.RunId == runId);
        _manifest.Runs.Add(metadata);
        SaveManifest();

        Plugin.LogSource.LogInfo($"Parked run '{runId}' at {path}.");
    }

    internal static CampaignSaveData? ReadParkedSave(string runId)
    {
        var path = GetParkedSavePath(runId);
        if (!File.Exists(path))
        {
            return null;
        }

        var json = File.ReadAllText(path);
        return JsonUtility.FromJson<CampaignSaveData>(json);
    }

    internal static void DeleteParkedRun(string runId)
    {
        EnsureLoaded();

        var path = GetParkedSavePath(runId);
        if (File.Exists(path))
        {
            File.Delete(path);
        }

        _manifest.Runs.RemoveAll(entry => entry.RunId == runId);
        SaveManifest();

        Plugin.LogSource.LogInfo($"Retired parked run '{runId}'.");
    }

    private static string GetRunsDirectory() =>
        Path.Combine(Application.persistentDataPath, "TTModRuns");

    private static string GetParkedSavePath(string runId) =>
        Path.Combine(GetRunsDirectory(), $"{runId}.json");

    private static string GetManifestPath() =>
        Path.Combine(GetRunsDirectory(), ManifestFileName);

    private static void LoadManifest()
    {
        var path = GetManifestPath();
        if (!File.Exists(path))
        {
            _manifest = new RunSlotManifest();
            return;
        }

        try
        {
            var json = File.ReadAllText(path);
            _manifest = JsonUtility.FromJson<RunSlotManifest>(json) ?? new RunSlotManifest();
            _manifest.Runs ??= new List<RunSlotMetadata>();
        }
        catch (Exception ex)
        {
            Plugin.LogSource.LogWarning($"Failed to load run manifest: {ex.Message}");
            _manifest = new RunSlotManifest();
        }
    }

    private static void SaveManifest()
    {
        Directory.CreateDirectory(GetRunsDirectory());
        var json = JsonUtility.ToJson(_manifest, prettyPrint: true);
        File.WriteAllText(GetManifestPath(), json);
    }

    [Serializable]
    private sealed class RunSlotManifest
    {
        public List<RunSlotMetadata> Runs = new();
    }

    [Serializable]
    private sealed class RunSlotMetadata
    {
        public string RunId = string.Empty;
        public string SavedUtc = string.Empty;
        public int HeroId;
        public string HeroName = string.Empty;
        public string Difficulty = string.Empty;
        public int BookNumber;
        public int Layer;
        public int ChaptersCompleted;
        public int Seed;
        public int GoldAmount;

        internal static RunSlotMetadata FromSave(CampaignSaveData save, string runId, string savedUtc)
        {
            var entry = RunCatalogService.ToEntry(save, RunStorageKind.Parked, savedUtc);
            return new RunSlotMetadata
            {
                RunId = runId,
                SavedUtc = savedUtc,
                HeroId = entry.HeroId,
                HeroName = entry.HeroName,
                Difficulty = entry.Difficulty,
                BookNumber = entry.BookNumber,
                Layer = entry.Layer,
                ChaptersCompleted = entry.ChaptersCompleted,
                Seed = entry.Seed,
                GoldAmount = entry.GoldAmount,
            };
        }

        internal RunListEntry ToEntry() =>
            new(
                RunId,
                RunStorageKind.Parked,
                HeroName,
                HeroId,
                Difficulty,
                BookNumber,
                Layer,
                ChaptersCompleted,
                Seed,
                GoldAmount,
                SavedUtc,
                RunCatalogService.ResolveLoopCount(RunId));
    }
}
