using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Memori.SaveData;
using TJ;
using TTMod.SaveGameManager.Models;
using UnityEngine;

namespace TTMod.SaveGameManager.Services;

internal static class RunCatalogService
{
    internal static IReadOnlyList<RunListEntry> GetSavedRuns()
    {
        RunSlotStore.EnsureLoaded();
        EndlessCampaignCompat.InvalidateCache();

        var entries = new List<RunListEntry>();

        if (SaveDataHandler.CampaignSaveExists())
        {
            var active = SaveDataHandler.Load();
            if (active != null && !active.blank)
            {
                entries.Add(ToEntry(active, RunStorageKind.Active, DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture)));
            }
        }

        foreach (var parked in RunSlotStore.GetParkedRuns())
        {
            if (entries.Exists(entry => entry.RunId == parked.RunId))
            {
                continue;
            }

            entries.Add(parked);
        }

        entries.Sort((left, right) =>
        {
            if (left.IsActive != right.IsActive)
            {
                return left.IsActive ? -1 : 1;
            }

            return string.Compare(right.SavedUtc, left.SavedUtc, StringComparison.Ordinal);
        });

        return entries;
    }

    internal static bool HasAnySavedRuns() => GetSavedRuns().Count > 0;

    internal static void LoadRun(RunListEntry entry, TJ.MainMenu.MainMenu mainMenu)
    {
        if (entry.IsActive)
        {
            mainMenu.LoadMapScene();
            return;
        }

        var save = RunSlotStore.ReadParkedSave(entry.RunId);
        if (save == null)
        {
            throw new InvalidOperationException($"Parked run '{entry.RunId}' could not be loaded.");
        }

        if (SaveDataHandler.CampaignSaveExists())
        {
            RunSlotStore.ParkActiveRun();
        }

        SaveDataHandler.SaveCampaign(save);
        mainMenu.LoadMapScene();
    }

    internal static void RetireRun(RunListEntry entry)
    {
        if (entry.IsActive)
        {
            SaveDataHandler.DeleteCampaignSave();
            return;
        }

        RunSlotStore.DeleteParkedRun(entry.RunId);
    }

    internal static void PrepareForNewRun()
    {
        if (!SaveDataHandler.CampaignSaveExists())
        {
            return;
        }

        RunSlotStore.ParkActiveRun();
        SaveDataHandler.DeleteCampaignSave();
    }

    internal static RunListEntry ToEntry(CampaignSaveData save, RunStorageKind storageKind, string savedUtc)
    {
        var hero = HeroData.GetHeroByID(save.heroID);
        var heroName = string.IsNullOrWhiteSpace(hero.HeroName)
            ? $"Hero {save.heroID + 1}"
            : hero.HeroName;
        var runId = ResolveRunId(save);

        return new RunListEntry(
            runId,
            storageKind,
            heroName,
            save.heroID,
            save.difficultyLevel.ToString(),
            save.bookNumber,
            save.activeMapLayer + 1,
            save.RunStats.chaptersCompleted,
            save.seed,
            save.goldAmount,
            savedUtc,
            ResolveLoopCount(runId));
    }

    internal static int? ResolveLoopCount(string runId)
    {
        if (!EndlessCampaignCompat.TryGetLoopCount(runId, out var loops))
        {
            return null;
        }

        return loops;
    }

    internal static string ResolveRunId(CampaignSaveData save)
    {
        if (save.runUUID != Guid.Empty)
        {
            return save.runUUID.ToString("N");
        }

        return $"legacy-seed-{save.seed}";
    }

    internal static string FormatSavedUtc(string savedUtc)
    {
        if (DateTime.TryParse(savedUtc, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var parsed))
        {
            return parsed.ToLocalTime().ToString("g");
        }

        return savedUtc;
    }
}
