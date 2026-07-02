using HarmonyLib;
using TJ.MainMenu;
using TTMod.SaveGameManager.UI;

namespace TTMod.SaveGameManager.Patches;

[HarmonyPatch(typeof(MainMenu), "CheckForCampaignSaveData")]
internal static class MainMenuCheckForCampaignSaveDataPatch
{
    internal static bool Prefix(MainMenu __instance)
    {
        if (!SaveGameManagerSettings.Enabled.Value)
        {
            return true;
        }

        MainMenuRunButtonsController.Apply(__instance);
        return false;
    }
}

[HarmonyPatch(typeof(MainMenu), "UpdateButtonText")]
internal static class MainMenuUpdateButtonTextPatch
{
    internal static void Postfix(MainMenu __instance)
    {
        if (!SaveGameManagerSettings.Enabled.Value)
        {
            return;
        }

        MainMenuRunButtonsController.RefreshLabels(__instance);
    }
}
