using HarmonyLib;
using TMPro;
using TJ.MainMenu;
using TTMod.SaveGameManager.Services;
using UnityEngine;
using UnityEngine.UI;

namespace TTMod.SaveGameManager.UI;

internal static class MainMenuRunButtonsController
{
    private static GameObject? _loadRunButtonObject;
    private static bool _installed;

    internal static void ResetInstallFlag()
    {
        _installed = false;
    }

    internal static void TryInstall()
    {
        if (_installed || !SaveGameManagerSettings.Enabled.Value)
        {
            return;
        }

        var mainMenu = Object.FindFirstObjectByType<MainMenu>();
        if (mainMenu == null)
        {
            return;
        }

        Apply(mainMenu);
    }

    internal static void Apply(MainMenu mainMenu)
    {
        if (!SaveGameManagerSettings.Enabled.Value)
        {
            return;
        }

        var playButton = AccessTools.Field(typeof(MainMenu), "playPanelButton")?.GetValue(mainMenu) as Button;
        var customBattleButton = AccessTools.Field(typeof(MainMenu), "customBattleButton")?.GetValue(mainMenu) as Button;
        var abandonButton = AccessTools.Field(typeof(MainMenu), "abandonRunButton")?.GetValue(mainMenu) as Button;
        if (playButton == null)
        {
            return;
        }

        if (abandonButton != null)
        {
            abandonButton.gameObject.SetActive(false);
        }

        WireNewRunButton(playButton, mainMenu);
        EnsureLoadRunButton(playButton, customBattleButton, mainMenu);

        _installed = true;
        Plugin.LogSource.LogInfo("Save Game Manager main menu buttons installed.");
    }

    internal static void RefreshLabels(MainMenu mainMenu)
    {
        if (!SaveGameManagerSettings.Enabled.Value)
        {
            return;
        }

        var playButton = AccessTools.Field(typeof(MainMenu), "playPanelButton")?.GetValue(mainMenu) as Button;
        if (playButton != null)
        {
            SetButtonLabel(playButton, SaveGameManagerSettings.NewRunButtonLabel.Value);
        }

        if (_loadRunButtonObject != null)
        {
            var loadButton = _loadRunButtonObject.GetComponent<Button>();
            if (loadButton != null)
            {
                SetButtonLabel(loadButton, SaveGameManagerSettings.LoadRunButtonLabel.Value);
            }
        }
    }

    private static void WireNewRunButton(Button playButton, MainMenu mainMenu)
    {
        playButton.onClick.RemoveAllListeners();
        playButton.onClick.AddListener(() =>
        {
            RunCatalogService.PrepareForNewRun();
            mainMenu.SwitchToPlayPanel();
            MainMenuRunButtonsController.ResetInstallFlag();
        });

        SetButtonLabel(playButton, SaveGameManagerSettings.NewRunButtonLabel.Value);
    }

    private static void EnsureLoadRunButton(Button playTemplate, Button? customBattleTemplate, MainMenu mainMenu)
    {
        if (_loadRunButtonObject != null)
        {
            Object.Destroy(_loadRunButtonObject);
            _loadRunButtonObject = null;
        }

        var cloneTemplate = customBattleTemplate ?? playTemplate;
        _loadRunButtonObject = Object.Instantiate(cloneTemplate.gameObject, playTemplate.transform.parent);
        _loadRunButtonObject.name = "TTMod_LoadRunButton";
        ApplyLoadRunLayout(playTemplate, customBattleTemplate);

        _loadRunButtonObject.SetActive(true);

        var loadButton = _loadRunButtonObject.GetComponent<Button>();
        loadButton.onClick.RemoveAllListeners();
        loadButton.onClick.AddListener(() => LoadRunOverlay.Open(mainMenu));
        SetButtonLabel(loadButton, SaveGameManagerSettings.LoadRunButtonLabel.Value);
    }

    private static void ApplyLoadRunLayout(Button playTemplate, Button? customBattleTemplate)
    {
        if (_loadRunButtonObject == null)
        {
            return;
        }

        var playRect = playTemplate.GetComponent<RectTransform>();
        var loadRect = _loadRunButtonObject.GetComponent<RectTransform>();
        var styleRect = customBattleTemplate?.GetComponent<RectTransform>() ?? playRect;

        loadRect.SetParent(playRect.parent, false);
        loadRect.anchorMin = styleRect.anchorMin;
        loadRect.anchorMax = styleRect.anchorMax;
        loadRect.pivot = styleRect.pivot;
        loadRect.sizeDelta = styleRect.sizeDelta;
        loadRect.localScale = styleRect.localScale;
        loadRect.SetSiblingIndex(playRect.GetSiblingIndex() + 1);

        var parentRect = playRect.parent as RectTransform;
        var parentLayout = parentRect?.GetComponent<VerticalLayoutGroup>();
        if (parentLayout != null && parentRect != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(parentRect);
            return;
        }

        if (customBattleTemplate != null)
        {
            var customBattleRect = customBattleTemplate.GetComponent<RectTransform>();
            var step = customBattleRect.anchoredPosition - playRect.anchoredPosition;
            loadRect.anchoredPosition = playRect.anchoredPosition + step;
            return;
        }

        loadRect.anchoredPosition = playRect.anchoredPosition + new Vector2(0f, -56f);
    }

    private static void SetButtonLabel(Button button, string label)
    {
        var text = button.GetComponentInChildren<TMP_Text>(true);
        if (text != null)
        {
            text.text = label;
        }
    }
}
