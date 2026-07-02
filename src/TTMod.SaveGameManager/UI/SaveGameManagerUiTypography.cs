using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TTMod.SaveGameManager.UI;

internal static class SaveGameManagerUiTypography
{
    internal static TMP_Text? FindSampleText()
    {
        return Object.FindFirstObjectByType<TMP_Text>();
    }

    internal static TMP_Text CreateText(
        Transform parent,
        TMP_Text? sample,
        string name,
        int fontSize,
        TextAlignmentOptions alignment = TextAlignmentOptions.TopLeft)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);

        var rect = go.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(400f, 32f);

        var text = go.AddComponent<TextMeshProUGUI>();
        if (sample != null)
        {
            text.font = sample.font;
            text.fontSharedMaterial = sample.fontSharedMaterial;
        }

        text.fontSize = fontSize;
        text.alignment = alignment;
        text.color = Color.white;
        text.raycastTarget = false;
        text.textWrappingMode = TextWrappingModes.Normal;
        return text;
    }

    internal static Button CreateButton(
        Transform parent,
        TMP_Text? sample,
        string name,
        string label,
        Vector2 size)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);

        var rect = go.AddComponent<RectTransform>();
        rect.sizeDelta = size;

        var image = go.AddComponent<Image>();
        image.color = new Color(0.12f, 0.14f, 0.18f, 0.92f);

        var button = go.AddComponent<Button>();
        var colors = button.colors;
        colors.normalColor = image.color;
        colors.highlightedColor = new Color(0.2f, 0.24f, 0.3f, 0.95f);
        colors.pressedColor = new Color(0.08f, 0.1f, 0.14f, 0.98f);
        colors.selectedColor = colors.highlightedColor;
        button.colors = colors;

        var labelGo = new GameObject("Label");
        labelGo.transform.SetParent(go.transform, false);
        var labelRect = labelGo.AddComponent<RectTransform>();
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;

        var text = labelGo.AddComponent<TextMeshProUGUI>();
        if (sample != null)
        {
            text.font = sample.font;
            text.fontSharedMaterial = sample.fontSharedMaterial;
        }

        text.fontSize = 16;
        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.white;
        text.text = label;
        text.raycastTarget = false;

        return button;
    }
}
