using TMPro;
using TJ.MainMenu;
using UnityEngine;

namespace TTMod.SaveGameManager.UI;

internal static class LoadRunOverlay
{
    private static bool _attached;

    internal static bool IsAttached => _attached;

    internal static LoadRunPanelController? Panel { get; private set; }

    internal static void TryCreate()
    {
        if (_attached)
        {
            return;
        }

        var sample = SaveGameManagerUiTypography.FindSampleText();
        if (sample == null)
        {
            return;
        }

        EnsureEventSystem();

        var root = new GameObject("TabletopTavernSaveGameManagerOverlay");
        Object.DontDestroyOnLoad(root);

        var canvasGo = new GameObject("Canvas");
        canvasGo.transform.SetParent(root.transform, false);

        var canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = short.MaxValue - 6;

        var scaler = canvasGo.AddComponent<UnityEngine.UI.CanvasScaler>();
        scaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        canvasGo.AddComponent<UnityEngine.UI.GraphicRaycaster>();

        Panel = canvasGo.AddComponent<LoadRunPanelController>();
        Panel.Initialize(sample);

        _attached = true;
        Plugin.LogSource.LogInfo("Save Game Manager overlay created.");
    }

    internal static void Open(MainMenu mainMenu)
    {
        if (!IsAttached)
        {
            TryCreate();
        }

        if (Panel == null)
        {
            Plugin.LogSource.LogWarning("Load Run panel is not available.");
            return;
        }

        Panel.Open(mainMenu);
    }

    private static void EnsureEventSystem()
    {
        if (Object.FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() != null)
        {
            return;
        }

        var eventSystemGo = new GameObject("SaveGameManagerEventSystem");
        Object.DontDestroyOnLoad(eventSystemGo);
        eventSystemGo.AddComponent<UnityEngine.EventSystems.EventSystem>();
        eventSystemGo.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
    }
}
