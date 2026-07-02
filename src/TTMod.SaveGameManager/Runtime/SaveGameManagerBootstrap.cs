using TTMod.Core.Lifecycle;
using TTMod.SaveGameManager.Services;
using TTMod.SaveGameManager.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TTMod.SaveGameManager.Runtime;

internal sealed class SaveGameManagerBootstrap : MonoBehaviour
{
    private int _frames;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
        TtLifecycle.MainMenuEntered += OnMainMenuEntered;
        RunSlotStore.EnsureLoaded();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        TtLifecycle.MainMenuEntered -= OnMainMenuEntered;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        TryCreateOverlay();
        if (TtLifecycle.IsMainMenu)
        {
            MainMenuRunButtonsController.ResetInstallFlag();
            MainMenuRunButtonsController.TryInstall();
        }
    }

    private void OnMainMenuEntered()
    {
        MainMenuRunButtonsController.ResetInstallFlag();
        MainMenuRunButtonsController.TryInstall();
    }

    private void Update()
    {
        if (!SaveGameManagerSettings.Enabled.Value)
        {
            return;
        }

        if (!LoadRunOverlay.IsAttached)
        {
            _frames++;
            if (_frames is 10 or 30 or 120 or 300)
            {
                TryCreateOverlay();
            }
        }

        if (TtLifecycle.IsMainMenu)
        {
            MainMenuRunButtonsController.TryInstall();
        }
    }

    private static void TryCreateOverlay()
    {
        if (!SaveGameManagerSettings.Enabled.Value)
        {
            return;
        }

        LoadRunOverlay.TryCreate();
    }
}
