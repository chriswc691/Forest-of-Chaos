#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

public class TextToTextMeshPro : Editor
{
    public class TextMeshProSettings
    {
        public bool Enabled;
        public FontStyles FontStyle;
        public float FontSize;
        public float FontSizeMin;
        public float FontSizeMax;
        public float LineSpacing;
        public bool EnableRichText;
        public bool EnableAutoSizing;
        public TextAlignmentOptions TextAlignmentOptions;
        public bool WrappingEnabled;
        public TextOverflowModes TextOverflowModes;
        public string Text;
        public Color Color;
        public bool RayCastTarget;
    }

    [MenuItem("Tools/Text To TMP (Single)", false)]
    static void SelectionToTextMeshPro()
    {
        if (IsDefaultFontMissing())
        {
            return;
        }

        foreach (GameObject gameObject in Selection.gameObjects)
        {
            Text text = gameObject.GetComponent<Text>();
            if (text != null)
            {
                ConvertTextToTextMeshPro(text);
            }
        }
    }

    [MenuItem("Tools/Text To TMP (Recursive)", false)]
    static void SelectionToTextMeshProRecursive()
    {
        if (IsDefaultFontMissing())
        {
            return;
        }

        SelectionToTextMeshPro();
        foreach (GameObject gameObject in Selection.gameObjects)
        {
            foreach (Text text in gameObject.GetComponentsInChildren<Text>())
            {
                ConvertTextToTextMeshPro(text);
            }
        }
    }

    static bool IsDefaultFontMissing()
    {
        if (TMPro.TMP_Settings.defaultFontAsset == null)
        {
            EditorUtility.DisplayDialog("Default Font Missing", "Assign a default font asset in project settings!", "Ok", "");
            return true;
        }
        return false;
    }

    static void ConvertTextToTextMeshPro(Text text)
    {
        GameObject parent = text.gameObject;
        Vector2 sizeDelta = parent.GetComponent<RectTransform>().sizeDelta;
        TextMeshProSettings settings = GetTextMeshProSettings(text);
        Object.DestroyImmediate(text as Object, false);

        TextMeshProUGUI tmp = parent.AddComponent<TextMeshProUGUI>();
        tmp.enabled = settings.Enabled;
        tmp.fontStyle = settings.FontStyle;
        tmp.fontSize = settings.FontSize;
        tmp.fontSizeMin = settings.FontSizeMin;
        tmp.fontSizeMax = settings.FontSizeMax;
        tmp.lineSpacing = settings.LineSpacing;
        tmp.richText = settings.EnableRichText;
        tmp.enableAutoSizing = settings.EnableAutoSizing;
        tmp.alignment = settings.TextAlignmentOptions;
        tmp.enableWordWrapping = settings.WrappingEnabled;
        tmp.overflowMode = settings.TextOverflowModes;
        tmp.text = settings.Text;
        tmp.color = settings.Color;
        tmp.raycastTarget = settings.RayCastTarget;

        parent.GetComponent<RectTransform>().sizeDelta = sizeDelta;
        Debug.Log("Converted Text to TextMeshPro on " + parent.name);
    }

    static TextMeshProSettings GetTextMeshProSettings(Text text)
    {
        return new TextMeshProSettings
        {
            Enabled = text.enabled,
            FontStyle = FontStyleToFontStyles(text.fontStyle),
            FontSize = text.fontSize,
            FontSizeMin = text.resizeTextMinSize,
            FontSizeMax = text.resizeTextMaxSize,
            LineSpacing = text.lineSpacing,
            EnableRichText = text.supportRichText,
            EnableAutoSizing = text.resizeTextForBestFit,
            TextAlignmentOptions = TextAnchorToTextAlignmentOptions(text.alignment),
            WrappingEnabled = HorizontalWrapModeToBool(text.horizontalOverflow),
            TextOverflowModes = VerticalWrapModeToTextOverflowModes(text.verticalOverflow),
            Text = text.text,
            Color = text.color,
            RayCastTarget = text.raycastTarget
        };
    }

    static bool HorizontalWrapModeToBool(HorizontalWrapMode overflow)
    {
        return overflow == HorizontalWrapMode.Wrap;
    }

    static TextOverflowModes VerticalWrapModeToTextOverflowModes(VerticalWrapMode verticalOverflow)
    {
        return verticalOverflow == VerticalWrapMode.Truncate ? TextOverflowModes.Truncate : TextOverflowModes.Overflow;
    }

    static FontStyles FontStyleToFontStyles(FontStyle fontStyle)
    {
        switch (fontStyle)
        {
            case FontStyle.Normal:
                return FontStyles.Normal;

            case FontStyle.Bold:
                return FontStyles.Bold;

            case FontStyle.Italic:
                return FontStyles.Italic;

            case FontStyle.BoldAndItalic:
                return FontStyles.Bold | FontStyles.Italic;
        }

        Debug.LogWarning("Unhandled font style " + fontStyle);
        return FontStyles.Normal;
    }

    static TextAlignmentOptions TextAnchorToTextAlignmentOptions(TextAnchor textAnchor)
    {
        switch (textAnchor)
        {
            case TextAnchor.UpperLeft:
                return TextAlignmentOptions.TopLeft;

            case TextAnchor.UpperCenter:
                return TextAlignmentOptions.Top;

            case TextAnchor.UpperRight:
                return TextAlignmentOptions.TopRight;

            case TextAnchor.MiddleLeft:
                return TextAlignmentOptions.Left;

            case TextAnchor.MiddleCenter:
                return TextAlignmentOptions.Center;

            case TextAnchor.MiddleRight:
                return TextAlignmentOptions.Right;

            case TextAnchor.LowerLeft:
                return TextAlignmentOptions.BottomLeft;

            case TextAnchor.LowerCenter:
                return TextAlignmentOptions.Bottom;

            case TextAnchor.LowerRight:
                return TextAlignmentOptions.BottomRight;
        }

        Debug.LogWarning("Unhandled text anchor " + textAnchor);
        return TextAlignmentOptions.TopLeft;
    }
}

#endif