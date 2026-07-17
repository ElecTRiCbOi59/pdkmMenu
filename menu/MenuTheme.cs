using UnityEngine;

internal static class MenuTheme
{
    public static readonly Color WindowBackground = new(0.045f, 0.052f, 0.068f, 0.985f);
    public static readonly Color SidebarBackground = new(0.065f, 0.075f, 0.096f, 1f);
    public static readonly Color Surface = new(0.105f, 0.118f, 0.15f, 1f);
    public static readonly Color SurfaceHover = new(0.145f, 0.162f, 0.205f, 1f);
    public static readonly Color InputSurface = new(0.035f, 0.042f, 0.056f, 1f);
    public static readonly Color InputFocused = new(0.055f, 0.067f, 0.09f, 1f);
    public static readonly Color Accent = new(0.25f, 0.47f, 0.95f, 1f);
    public static readonly Color AccentHover = new(0.32f, 0.54f, 1f, 1f);
    public static readonly Color Danger = new(0.72f, 0.22f, 0.25f, 1f);
    public static readonly Color Text = new(0.95f, 0.96f, 0.98f, 1f);
    public static readonly Color MutedText = new(0.62f, 0.66f, 0.73f, 1f);
    public static readonly Color Border = new(0.18f, 0.205f, 0.26f, 1f);
    public static readonly Color HostStatus = new(0.16f, 0.58f, 0.38f, 1f);
    public static readonly Color CoHostStatus = new(0.55f, 0.35f, 0.92f, 1f);
    public static readonly Color ClientStatus = new(0.25f, 0.47f, 0.95f, 1f);

    public const float WindowWidth = 1240f;
    public const float WindowHeight = 800f;
    public const float HeaderHeight = 72f;
    public const float SidebarWidth = 250f;
    public const float FooterHeight = 52f;
    public const float OuterPadding = 18f;
    public const float ControlHeight = 40f;
    public const float SliderCardHeight = 48f;
    public const float ControlGap = 12f;
    public const float ColumnGap = 18f;
    public const float PageBottomPadding = 24f;

    private const int TextureSize = 32;
    private const int WindowRadius = 14;
    private const int ControlRadius = 10;

    private static Texture2D windowTexture;
    private static Texture2D sidebarTexture;
    private static Texture2D surfaceTexture;
    private static Texture2D surfaceHoverTexture;
    private static Texture2D inputTexture;
    private static Texture2D inputFocusedTexture;
    private static Texture2D accentTexture;
    private static Texture2D accentHoverTexture;
    private static Texture2D dangerTexture;
    private static Texture2D borderTexture;
    private static Texture2D sliderTrackTexture;
    private static Texture2D sliderThumbTexture;
    private static Texture2D transparentTexture;
    private static Texture2D hostStatusTexture;
    private static Texture2D coHostStatusTexture;
    private static Texture2D clientStatusTexture;

    private static GUIStyle surfacePanelStyle;
    private static GUIStyle sidebarPanelStyle;
    private static GUIStyle borderPanelStyle;
    private static GUIStyle accentPanelStyle;

    public static GUIStyle WindowStyle { get; private set; }
    public static GUIStyle HeaderTitleStyle { get; private set; }
    public static GUIStyle HeaderSubtitleStyle { get; private set; }
    public static GUIStyle NavButtonStyle { get; private set; }
    public static GUIStyle NavButtonSelectedStyle { get; private set; }
    public static GUIStyle ButtonStyle { get; private set; }
    public static GUIStyle ActiveButtonStyle { get; private set; }
    public static GUIStyle ActiveStateLabelStyle { get; private set; }
    public static GUIStyle DisabledButtonStyle { get; private set; }
    public static GUIStyle SectionHeaderStyle { get; private set; }
    public static GUIStyle LabelStyle { get; private set; }
    public static GUIStyle ValueLabelStyle { get; private set; }
    public static GUIStyle MutedLabelStyle { get; private set; }
    public static GUIStyle TextFieldStyle { get; private set; }
    public static GUIStyle SliderInputStyle { get; private set; }
    public static GUIStyle FooterButtonStyle { get; private set; }
    public static GUIStyle SliderStyle { get; private set; }
    public static GUIStyle SliderThumbStyle { get; private set; }
    public static GUIStyle VerticalScrollbarStyle { get; private set; }
    public static GUIStyle ScrollViewBackgroundStyle { get; private set; }
    public static GUIStyle HostStatusStyle { get; private set; }
    public static GUIStyle CoHostStatusStyle { get; private set; }
    public static GUIStyle ClientStatusStyle { get; private set; }

    public static void EnsureInitialised()
    {
        if (WindowStyle != null)
        {
            return;
        }

        windowTexture = CreateRoundedTexture(WindowBackground, WindowRadius);
        sidebarTexture = CreateRoundedTexture(SidebarBackground, ControlRadius);
        surfaceTexture = CreateRoundedTexture(Surface, ControlRadius);
        surfaceHoverTexture = CreateRoundedTexture(SurfaceHover, ControlRadius);
        inputTexture = CreateRoundedTexture(InputSurface, ControlRadius);
        inputFocusedTexture = CreateRoundedTexture(InputFocused, ControlRadius);
        accentTexture = CreateRoundedTexture(Accent, ControlRadius);
        accentHoverTexture = CreateRoundedTexture(AccentHover, ControlRadius);
        dangerTexture = CreateRoundedTexture(Danger, ControlRadius);
        borderTexture = CreateTexture(Border);
        sliderTrackTexture = CreateRoundedTexture(new Color(0.19f, 0.22f, 0.29f, 1f), 4);
        sliderThumbTexture = CreateRoundedTexture(Accent, 8);
        transparentTexture = CreateTexture(new Color(0f, 0f, 0f, 0f));
        // Use the maximum radius so role indicators read as true status pills.
        hostStatusTexture = CreateRoundedTexture(HostStatus, TextureSize / 2);
        coHostStatusTexture = CreateRoundedTexture(CoHostStatus, TextureSize / 2);
        clientStatusTexture = CreateRoundedTexture(ClientStatus, TextureSize / 2);

        WindowStyle = new GUIStyle(GUI.skin.window)
        {
            border = new RectOffset(WindowRadius, WindowRadius, WindowRadius, WindowRadius),
            padding = new RectOffset(0, 0, 0, 0)
        };
        WindowStyle.normal.background = windowTexture;
        WindowStyle.onNormal.background = windowTexture;

        surfacePanelStyle = CreatePanelStyle(surfaceTexture, ControlRadius);
        sidebarPanelStyle = CreatePanelStyle(sidebarTexture, ControlRadius);
        borderPanelStyle = CreatePanelStyle(borderTexture, 0);
        accentPanelStyle = CreatePanelStyle(accentHoverTexture, ControlRadius);

        HeaderTitleStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 26,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleLeft
        };
        HeaderTitleStyle.normal.textColor = Text;

        HeaderSubtitleStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 14,
            alignment = TextAnchor.MiddleLeft
        };
        HeaderSubtitleStyle.normal.textColor = MutedText;

        HostStatusStyle = CreateStatusPillStyle(hostStatusTexture);
        CoHostStatusStyle = CreateStatusPillStyle(coHostStatusTexture);
        ClientStatusStyle = CreateStatusPillStyle(clientStatusTexture);

        NavButtonStyle = CreateButtonStyle(surfaceTexture, surfaceHoverTexture, TextAnchor.MiddleLeft, 15);
        NavButtonStyle.padding = new RectOffset(16, 12, 0, 0);

        NavButtonSelectedStyle = CreateButtonStyle(accentTexture, accentHoverTexture, TextAnchor.MiddleLeft, 15);
        NavButtonSelectedStyle.fontStyle = FontStyle.Bold;
        NavButtonSelectedStyle.padding = new RectOffset(16, 12, 0, 0);

        ButtonStyle = CreateButtonStyle(surfaceTexture, surfaceHoverTexture, TextAnchor.MiddleCenter, 14);
        ActiveButtonStyle = CreateButtonStyle(accentTexture, accentHoverTexture, TextAnchor.MiddleCenter, 14);
        ActiveButtonStyle.fontStyle = FontStyle.Bold;
        ActiveButtonStyle.padding = new RectOffset(12, 12, 0, 0);

        ActiveStateLabelStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 10,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleRight
        };
        ActiveStateLabelStyle.normal.textColor = Color.white;

        DisabledButtonStyle = CreateButtonStyle(dangerTexture, dangerTexture, TextAnchor.MiddleCenter, 14);
        FooterButtonStyle = CreateButtonStyle(surfaceTexture, surfaceHoverTexture, TextAnchor.MiddleCenter, 13);

        SectionHeaderStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 13,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleLeft,
            padding = new RectOffset(2, 0, 0, 0)
        };
        SectionHeaderStyle.normal.textColor = AccentHover;

        LabelStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 14,
            alignment = TextAnchor.MiddleLeft,
            clipping = TextClipping.Clip
        };
        LabelStyle.normal.textColor = Text;

        ValueLabelStyle = new GUIStyle(LabelStyle)
        {
            fontSize = 13,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleRight
        };
        ValueLabelStyle.normal.textColor = AccentHover;

        MutedLabelStyle = new GUIStyle(LabelStyle)
        {
            fontSize = 12
        };
        MutedLabelStyle.normal.textColor = MutedText;

        TextFieldStyle = new GUIStyle(GUI.skin.textField)
        {
            fontSize = 15,
            alignment = TextAnchor.MiddleLeft,
            padding = new RectOffset(12, 12, 0, 0),
            border = new RectOffset(ControlRadius, ControlRadius, ControlRadius, ControlRadius)
        };
        TextFieldStyle.normal.background = inputTexture;
        TextFieldStyle.hover.background = inputFocusedTexture;
        TextFieldStyle.focused.background = inputFocusedTexture;
        TextFieldStyle.normal.textColor = Text;
        TextFieldStyle.hover.textColor = Text;
        TextFieldStyle.focused.textColor = Text;

        SliderInputStyle = new GUIStyle(TextFieldStyle)
        {
            fontSize = 12,
            alignment = TextAnchor.MiddleCenter,
            padding = new RectOffset(6, 6, 0, 0)
        };

        SliderStyle = new GUIStyle(GUI.skin.horizontalSlider)
        {
            fixedHeight = 8f,
            border = new RectOffset(4, 4, 4, 4)
        };
        SliderStyle.normal.background = sliderTrackTexture;

        SliderThumbStyle = new GUIStyle(GUI.skin.horizontalSliderThumb)
        {
            fixedWidth = 18f,
            fixedHeight = 18f,
            border = new RectOffset(8, 8, 8, 8)
        };
        SliderThumbStyle.normal.background = sliderThumbTexture;
        SliderThumbStyle.hover.background = accentHoverTexture;
        SliderThumbStyle.active.background = accentHoverTexture;
        VerticalScrollbarStyle = new GUIStyle(GUI.skin.verticalScrollbar)
        {
            fixedWidth = 8f,
            margin = new RectOffset(5, 0, 3, 3)
        };
        VerticalScrollbarStyle.normal.background = transparentTexture;
        VerticalScrollbarStyle.hover.background = transparentTexture;
        VerticalScrollbarStyle.active.background = transparentTexture;

        GUIStyle scrollbarThumb = new GUIStyle(GUI.skin.verticalScrollbarThumb)
        {
            fixedWidth = 8f,
            border = new RectOffset(4, 4, 4, 4)
        };
        scrollbarThumb.normal.background = accentTexture;
        scrollbarThumb.hover.background = accentHoverTexture;
        scrollbarThumb.active.background = accentHoverTexture;
        VerticalScrollbarStyle.onNormal.background = transparentTexture;
        VerticalScrollbarStyle.onHover.background = transparentTexture;
        VerticalScrollbarStyle.onActive.background = transparentTexture;

        // Unity takes the thumb style from the current skin for BeginScrollView.
        GUI.skin.verticalScrollbar = VerticalScrollbarStyle;
        GUI.skin.verticalScrollbarThumb = scrollbarThumb;

        ScrollViewBackgroundStyle = new GUIStyle(GUI.skin.scrollView);
        ScrollViewBackgroundStyle.normal.background = transparentTexture;
        ScrollViewBackgroundStyle.hover.background = transparentTexture;
        ScrollViewBackgroundStyle.active.background = transparentTexture;
    }

    public static void DrawSlider(Rect rect, float normalisedValue, bool enabled)
    {
        EnsureInitialised();

        Rect trackRect = new(
            rect.x,
            rect.center.y - 4f,
            rect.width,
            8f
        );
        GUI.Box(trackRect, GUIContent.none, SliderStyle);

        const float thumbSize = 18f;
        float thumbX = Mathf.Lerp(
            rect.x,
            rect.xMax - thumbSize,
            Mathf.Clamp01(normalisedValue)
        );
        Rect thumbRect = new(
            thumbX,
            rect.center.y - (thumbSize / 2f),
            thumbSize,
            thumbSize
        );

        Color previousColor = GUI.color;
        GUI.color = enabled ? Color.white : new Color(1f, 1f, 1f, 0.4f);
        GUI.Box(thumbRect, GUIContent.none, SliderThumbStyle);
        GUI.color = previousColor;
    }

    public static void DrawAccentBar(Rect rect)
    {
        EnsureInitialised();
        GUI.Box(rect, GUIContent.none, accentPanelStyle);
    }

    public static void DrawPanel(Rect rect, Color color)
    {
        EnsureInitialised();

        GUIStyle style = color == SidebarBackground
            ? sidebarPanelStyle
            : color == Border
                ? borderPanelStyle
                : surfacePanelStyle;

        GUI.Box(rect, GUIContent.none, style);
    }

    private static GUIStyle CreateStatusPillStyle(Texture2D background)
    {
        GUIStyle style = new(GUI.skin.label)
        {
            fontSize = 12,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter,
            padding = new RectOffset(12, 12, 0, 0),
            border = new RectOffset(TextureSize / 2, TextureSize / 2, TextureSize / 2, TextureSize / 2)
        };

        style.normal.background = background;
        style.normal.textColor = Color.white;
        return style;
    }

    private static GUIStyle CreateButtonStyle(Texture2D normal, Texture2D hover, TextAnchor alignment, int fontSize)
    {
        GUIStyle style = new(GUI.skin.button)
        {
            fontSize = fontSize,
            alignment = alignment,
            clipping = TextClipping.Clip,
            wordWrap = false,
            border = new RectOffset(ControlRadius, ControlRadius, ControlRadius, ControlRadius)
        };

        style.normal.background = normal;
        style.hover.background = hover;
        style.active.background = hover;
        style.focused.background = normal;
        style.normal.textColor = Text;
        style.hover.textColor = Text;
        style.active.textColor = Text;
        style.focused.textColor = Text;

        return style;
    }

    private static GUIStyle CreatePanelStyle(Texture2D texture, int radius)
    {
        GUIStyle style = new(GUI.skin.box)
        {
            border = new RectOffset(radius, radius, radius, radius),
            padding = new RectOffset(0, 0, 0, 0)
        };
        style.normal.background = texture;
        return style;
    }

    private static Texture2D CreateTexture(Color color)
    {
        Texture2D texture = new(1, 1, TextureFormat.RGBA32, false)
        {
            hideFlags = HideFlags.HideAndDontSave
        };

        texture.SetPixel(0, 0, color);
        texture.Apply();
        return texture;
    }

    private static Texture2D CreateRoundedTexture(Color color, int radius)
    {
        Texture2D texture = new(TextureSize, TextureSize, TextureFormat.RGBA32, false)
        {
            hideFlags = HideFlags.HideAndDontSave,
            wrapMode = TextureWrapMode.Clamp,
            filterMode = FilterMode.Bilinear
        };

        Color transparent = new(color.r, color.g, color.b, 0f);

        for (int y = 0; y < TextureSize; y++)
        {
            for (int x = 0; x < TextureSize; x++)
            {
                float nearestX = Mathf.Clamp(x, radius, TextureSize - radius - 1);
                float nearestY = Mathf.Clamp(y, radius, TextureSize - radius - 1);
                float distance = Vector2.Distance(new Vector2(x, y), new Vector2(nearestX, nearestY));
                float alpha = Mathf.Clamp01(radius + 0.5f - distance);

                texture.SetPixel(x, y, Color.Lerp(transparent, color, alpha));
            }
        }

        texture.Apply();
        return texture;
    }
}