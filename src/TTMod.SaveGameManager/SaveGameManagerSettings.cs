using BepInEx.Configuration;
using TTMod.Core.Config;

namespace TTMod.SaveGameManager;

internal static class SaveGameManagerSettings
{
    internal static ConfigEntry<bool> Enabled { get; private set; } = null!;
    internal static ConfigEntry<string> NewRunButtonLabel { get; private set; } = null!;
    internal static ConfigEntry<string> LoadRunButtonLabel { get; private set; } = null!;
    internal static ConfigEntry<string> RetireRunButtonLabel { get; private set; } = null!;

    internal static void Initialize(ConfigFile config)
    {
        Enabled = TtConfig.BindEnabled(config);
        NewRunButtonLabel = config.Bind(
            TtConfig.UiSection,
            "NewRunButtonLabel",
            "New Run",
            "Label for the main menu New Run button.");
        LoadRunButtonLabel = config.Bind(
            TtConfig.UiSection,
            "LoadRunButtonLabel",
            "Load Run",
            "Label for the main menu Load Run button.");
        RetireRunButtonLabel = config.Bind(
            TtConfig.UiSection,
            "RetireRunButtonLabel",
            "Retire Run",
            "Label for the retire action on each saved run row.");
    }
}
