using System.Collections.Generic;
using TMPro;
using TJ.MainMenu;
using TTMod.SaveGameManager.Models;
using TTMod.SaveGameManager.Services;
using UnityEngine;
using UnityEngine.UI;

namespace TTMod.SaveGameManager.UI;

internal sealed class LoadRunPanelController : MonoBehaviour
{
    private const float Margin = 18f;
    private const float RowHeight = 112f;
    private const float RowSpacing = 10f;

    private TMP_Text? _sample;
    private GameObject? _panelRoot;
    private TMP_Text? _headerText;
    private Transform? _listContent;
    private MainMenu? _mainMenu;

    internal void Initialize(TMP_Text sample)
    {
        _sample = sample;
        BuildPanel();
        SetOpen(false);
    }

    internal void Open(MainMenu mainMenu)
    {
        _mainMenu = mainMenu;
        SetOpen(true);
    }

    private void Update()
    {
        if (_panelRoot != null && _panelRoot.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            SetOpen(false);
        }
    }

    private void SetOpen(bool open)
    {
        if (_panelRoot != null)
        {
            _panelRoot.SetActive(open);
        }

        if (open)
        {
            Refresh();
        }
    }

    private void Refresh()
    {
        if (_headerText == null || _listContent == null)
        {
            return;
        }

        var runs = RunCatalogService.GetSavedRuns();
        _headerText.text = runs.Count == 0
            ? "<b>Load Run</b>\nNo saved runs yet. Start a New Run from the main menu."
            : $"<b>Load Run</b>\nSelect a run to continue. Retire removes it permanently.";

        for (var i = _listContent.childCount - 1; i >= 0; i--)
        {
            Destroy(_listContent.GetChild(i).gameObject);
        }

        foreach (var run in runs)
        {
            CreateRunRow(run);
        }

        var layout = _listContent.GetComponent<VerticalLayoutGroup>();
        if (layout != null)
        {
            var totalHeight = runs.Count * (RowHeight + RowSpacing);
            var rect = _listContent.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(rect.sizeDelta.x, Mathf.Max(totalHeight, 200f));
        }
    }

    private void CreateRunRow(RunListEntry run)
    {
        if (_listContent == null || _sample == null)
        {
            return;
        }

        var rowGo = new GameObject($"RunRow_{run.RunId}");
        rowGo.transform.SetParent(_listContent, false);

        var rowRect = rowGo.AddComponent<RectTransform>();
        rowRect.sizeDelta = new Vector2(0f, RowHeight);

        var rowLayout = rowGo.AddComponent<LayoutElement>();
        rowLayout.minHeight = RowHeight;
        rowLayout.preferredHeight = RowHeight;

        var rowImage = rowGo.AddComponent<Image>();
        rowImage.color = run.IsActive
            ? new Color(0.16f, 0.2f, 0.26f, 0.96f)
            : new Color(0.12f, 0.14f, 0.18f, 0.94f);

        var loadButton = rowGo.AddComponent<Button>();
        var loadColors = loadButton.colors;
        loadColors.normalColor = rowImage.color;
        loadColors.highlightedColor = new Color(0.22f, 0.27f, 0.34f, 0.98f);
        loadColors.pressedColor = new Color(0.1f, 0.12f, 0.16f, 0.98f);
        loadColors.selectedColor = loadColors.highlightedColor;
        loadButton.colors = loadColors;
        loadButton.onClick.AddListener(() => OnLoadRun(run));

        var details = SaveGameManagerUiTypography.CreateText(
            rowGo.transform,
            _sample,
            "Details",
            17,
            TextAlignmentOptions.MidlineLeft);
        details.raycastTarget = false;
        var detailsRect = details.rectTransform;
        detailsRect.anchorMin = new Vector2(0f, 0f);
        detailsRect.anchorMax = new Vector2(1f, 1f);
        detailsRect.offsetMin = new Vector2(16f, 12f);
        detailsRect.offsetMax = new Vector2(-150f, -12f);
        details.text = FormatRunDetails(run);

        var retireButton = SaveGameManagerUiTypography.CreateButton(
            rowGo.transform,
            _sample,
            "RetireRunButton",
            SaveGameManagerSettings.RetireRunButtonLabel.Value,
            new Vector2(120f, 36f));
        var retireRect = retireButton.GetComponent<RectTransform>();
        retireRect.anchorMin = new Vector2(1f, 0.5f);
        retireRect.anchorMax = new Vector2(1f, 0.5f);
        retireRect.pivot = new Vector2(1f, 0.5f);
        retireRect.anchoredPosition = new Vector2(-16f, 0f);
        retireButton.onClick.AddListener(() => OnRetireRun(run));
    }

    private static string FormatRunDetails(RunListEntry run)
    {
        var status = run.IsActive ? "Active run" : "Parked run";
        var saved = RunCatalogService.FormatSavedUtc(run.SavedUtc);
        var loops = run.LoopsCompleted.HasValue ? $"Loops {run.LoopsCompleted.Value}  |  " : string.Empty;

        return
            $"<b>{run.HeroName}</b>  |  {status}\n" +
            $"{loops}Book {run.BookNumber}  |  Layer {run.Layer}  |  Chapters {run.ChaptersCompleted}\n" +
            $"Difficulty {run.Difficulty}  |  Seed {run.Seed}  |  Gold {run.GoldAmount}\n" +
            $"Saved {saved}";
    }

    private void OnLoadRun(RunListEntry run)
    {
        if (_mainMenu == null)
        {
            Plugin.LogSource.LogWarning("Load Run: main menu reference missing.");
            return;
        }

        try
        {
            RunCatalogService.LoadRun(run, _mainMenu);
            SetOpen(false);
        }
        catch (System.Exception ex)
        {
            Plugin.LogSource.LogError($"Load Run failed: {ex}");
        }
    }

    private void OnRetireRun(RunListEntry run)
    {
        try
        {
            RunCatalogService.RetireRun(run);
            MainMenuRunButtonsController.ResetInstallFlag();
            MainMenuRunButtonsController.TryInstall();
            Refresh();
        }
        catch (System.Exception ex)
        {
            Plugin.LogSource.LogError($"Retire Run failed: {ex}");
        }
    }

    private void BuildPanel()
    {
        _panelRoot = new GameObject("SaveGameManagerLoadRunPanel");
        _panelRoot.transform.SetParent(transform, false);

        var panelRect = _panelRoot.AddComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        var backdrop = _panelRoot.AddComponent<Image>();
        backdrop.color = new Color(0f, 0f, 0f, 0.62f);
        backdrop.raycastTarget = true;

        var backdropButton = _panelRoot.AddComponent<Button>();
        backdropButton.targetGraphic = backdrop;
        var backdropColors = backdropButton.colors;
        backdropColors.normalColor = backdrop.color;
        backdropColors.highlightedColor = backdrop.color;
        backdropColors.pressedColor = backdrop.color;
        backdropColors.selectedColor = backdrop.color;
        backdropButton.colors = backdropColors;
        backdropButton.onClick.AddListener(() => SetOpen(false));

        var windowGo = new GameObject("Window");
        windowGo.transform.SetParent(_panelRoot.transform, false);
        var windowRect = windowGo.AddComponent<RectTransform>();
        windowRect.anchorMin = new Vector2(0.5f, 0.5f);
        windowRect.anchorMax = new Vector2(0.5f, 0.5f);
        windowRect.pivot = new Vector2(0.5f, 0.5f);
        windowRect.sizeDelta = new Vector2(860f, 620f);

        var windowImage = windowGo.AddComponent<Image>();
        windowImage.color = new Color(0.1f, 0.11f, 0.14f, 0.96f);
        windowImage.raycastTarget = true;

        var windowButton = windowGo.AddComponent<Button>();
        windowButton.targetGraphic = windowImage;
        var windowColors = windowButton.colors;
        windowColors.normalColor = windowImage.color;
        windowColors.highlightedColor = windowImage.color;
        windowColors.pressedColor = windowImage.color;
        windowColors.selectedColor = windowImage.color;
        windowButton.colors = windowColors;
        windowButton.onClick.AddListener(() => { });

        var header = SaveGameManagerUiTypography.CreateText(
            windowGo.transform,
            _sample,
            "Header",
            22,
            TextAlignmentOptions.TopLeft);
        var headerRect = header.rectTransform;
        headerRect.anchorMin = new Vector2(0f, 1f);
        headerRect.anchorMax = new Vector2(1f, 1f);
        headerRect.pivot = new Vector2(0.5f, 1f);
        headerRect.anchoredPosition = new Vector2(0f, -Margin);
        headerRect.sizeDelta = new Vector2(-Margin * 2f, 72f);
        headerRect.offsetMin = new Vector2(Margin, headerRect.offsetMin.y);
        headerRect.offsetMax = new Vector2(-Margin, headerRect.offsetMax.y);
        _headerText = header;

        var closeButton = SaveGameManagerUiTypography.CreateButton(
            windowGo.transform,
            _sample,
            "CloseButton",
            "Close",
            new Vector2(100f, 36f));
        var closeRect = closeButton.GetComponent<RectTransform>();
        closeRect.anchorMin = new Vector2(1f, 1f);
        closeRect.anchorMax = new Vector2(1f, 1f);
        closeRect.pivot = new Vector2(1f, 1f);
        closeRect.anchoredPosition = new Vector2(-Margin, -Margin);
        closeButton.onClick.AddListener(() => SetOpen(false));

        var scrollGo = new GameObject("Scroll");
        scrollGo.transform.SetParent(windowGo.transform, false);
        var scrollRectTransform = scrollGo.AddComponent<RectTransform>();
        scrollRectTransform.anchorMin = Vector2.zero;
        scrollRectTransform.anchorMax = Vector2.one;
        scrollRectTransform.offsetMin = new Vector2(Margin, Margin);
        scrollRectTransform.offsetMax = new Vector2(-Margin, -110f);

        var scroll = scrollGo.AddComponent<ScrollRect>();
        scroll.horizontal = false;
        scroll.movementType = ScrollRect.MovementType.Clamped;

        var viewportGo = new GameObject("Viewport");
        viewportGo.transform.SetParent(scrollGo.transform, false);
        var viewportRect = viewportGo.AddComponent<RectTransform>();
        viewportRect.anchorMin = Vector2.zero;
        viewportRect.anchorMax = Vector2.one;
        viewportRect.offsetMin = Vector2.zero;
        viewportRect.offsetMax = Vector2.zero;
        viewportGo.AddComponent<RectMask2D>();
        viewportGo.AddComponent<Image>().color = new Color(0f, 0f, 0f, 0.15f);

        var contentGo = new GameObject("Content");
        contentGo.transform.SetParent(viewportGo.transform, false);
        var contentRect = contentGo.AddComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0f, 1f);
        contentRect.anchorMax = new Vector2(1f, 1f);
        contentRect.pivot = new Vector2(0.5f, 1f);
        contentRect.anchoredPosition = Vector2.zero;
        contentRect.sizeDelta = new Vector2(0f, 400f);

        var verticalLayout = contentGo.AddComponent<VerticalLayoutGroup>();
        verticalLayout.spacing = RowSpacing;
        verticalLayout.padding = new RectOffset(0, 0, 0, 0);
        verticalLayout.childControlHeight = true;
        verticalLayout.childControlWidth = true;
        verticalLayout.childForceExpandHeight = false;
        verticalLayout.childForceExpandWidth = true;

        var fitter = contentGo.AddComponent<ContentSizeFitter>();
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        scroll.viewport = viewportRect;
        scroll.content = contentRect;
        _listContent = contentGo.transform;
    }
}
