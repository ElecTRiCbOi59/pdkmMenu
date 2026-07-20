using pdkmMenu;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class guiBase : MonoBehaviour
{
    public static Rect ContentRect;
    public static float CurrentPageContentHeight { get; private set; }

    public static void ResetPageMeasurement(float minimumHeight)
    {
        CurrentPageContentHeight = minimumHeight;
    }

    public Color CustomRed = new(0.5f, 0.2f, 0.2f, 1f);
    public Color CustomBlue = new(0.2f, 0.3f, 0.8f, 1f);
    public Color MenuColor;
    public Color TextColor = Color.white;

    public float ButtonWidthPercentage = 0.06f;
    public float ButtonHeightPercentage = 0.025f;
    public float PaddingYPercentage = 0.03f;
    public float XPercentage;
    public float YPercentage;
    public int ButtonIndex;
    public int CurrentColumn;
    public float ColumnWidthOffset = 0.07f;

    public float infoWidthPercentage = 0.06f;
    public float infoHeighthPercentage = 0.015f;
    public int InfoIndex;

    public int ContainerX;
    public int ContainterY;
    public int ContainterIndex_x;
    public int ContainterIndex_y;
    public int ContainterImgX = 75;
    public int ContainterImgY = 75;

    private const int MaxColumns = 3;
    private readonly Dictionary<string, string> sliderInputValues = new();
    private readonly float[] columnVerticalOffsets = new float[MaxColumns];
    private int gridRowLen;

    private float ColumnWidth
    {
        get
        {
            float gaps = MenuTheme.ColumnGap * (MaxColumns - 1);
            return (ContentRect.width - gaps) / MaxColumns;
        }
    }

    private int ColumnIndex => Mathf.Max(0, CurrentColumn - 1);

    private float CurrentX => ColumnIndex * (ColumnWidth + MenuTheme.ColumnGap);

    private float CurrentY =>
        (ButtonIndex * (MenuTheme.ControlHeight + MenuTheme.ControlGap))
        + columnVerticalOffsets[ColumnIndex];

    public float AddSlider(
        float minValue,
        float maxValue,
        float currentValue,
        string label = "",
        bool toggle = true,
        float step = 0f,
        bool allowInput = false
    )
    {
        MenuTheme.EnsureInitialised();

        Rect rect = GetControlRect(MenuTheme.SliderCardHeight);
        MenuTheme.DrawPanel(rect, MenuTheme.Surface);

        float value = Mathf.Clamp(currentValue, minValue, maxValue);
        float inputWidth = allowInput ? 70f : 0f;

        GUI.Label(
            new Rect(rect.x + 12f, rect.y + 3f, rect.width - 24f - inputWidth, 20f),
            label,
            toggle ? MenuTheme.LabelStyle : MenuTheme.MutedLabelStyle
        );

        if (allowInput)
        {
            value = DrawSliderInput(
                new Rect(rect.xMax - inputWidth - 10f, rect.y + 4f, inputWidth, 22f),
                value,
                minValue,
                maxValue,
                step,
                toggle
            );
        }

        Rect sliderRect = new(
            rect.x + 12f,
            rect.y + 29f,
            rect.width - 24f,
            16f
        );

        value = DrawSlider(
            sliderRect,
            value,
            minValue,
            maxValue,
            toggle
        );

        ButtonIndex++;
        return value;
    }

    public int AddSlider(
        int minValue,
        int maxValue,
        int currentValue,
        string label = "",
        bool toggle = true,
        bool allowInput = false
    )
    {
        return Mathf.RoundToInt(AddSlider(
            (float)minValue,
            maxValue,
            currentValue,
            label,
            toggle,
            1f,
            allowInput
        ));
    }

    public void AddButton(LKey key, UnityAction buttonAction, bool toggle = false)
    {
        AddButtonInternal(Localization.T(key), buttonAction, toggle);
    }

    public void AddButton(string rawName, UnityAction buttonAction, bool toggle = false)
    {
        AddButtonInternal(rawName, buttonAction, toggle);
    }

    public void AddVerticalSpace(float pixels)
    {
        columnVerticalOffsets[ColumnIndex] += Mathf.Max(0f, pixels);
    }

    public void AddSectionHeader(LKey key)
    {
        AddSectionHeader(Localization.T(key));
    }

    public void AddSectionHeader(string label)
    {
        MenuTheme.EnsureInitialised();

        Rect rect = GetControlRect(30f);
        GUI.Label(new Rect(rect.x, rect.y + 3f, rect.width, 24f), label.ToUpperInvariant(), MenuTheme.SectionHeaderStyle);
        MenuTheme.DrawPanel(new Rect(rect.x, rect.yMax - 2f, rect.width, 1f), MenuTheme.Border);
        ReportContentBottom(rect.yMax);
        ButtonIndex++;
        columnVerticalOffsets[ColumnIndex] -= 6f;
    }

    public string TextBox(string currentValue)
    {
        MenuTheme.EnsureInitialised();

        Rect rect = GetControlRect();
        currentValue = GUI.TextField(rect, currentValue, 25, MenuTheme.TextFieldStyle);

        ButtonIndex++;
        return currentValue;
    }

    public int IntTextBox(int currentValue)
    {
        string input = TextBox(currentValue.ToString());
        return int.TryParse(input, out int result) ? result : currentValue;
    }

    public void AddButtonList(
        Dictionary<string, Tuple<bool, Action>> keyValuePairs,
        ref Vector2 scrollPosition,
        int index = 0,
        int RowDisplayCount = 10
    )
    {
        MenuTheme.EnsureInitialised();

        float x = CurrentX + (index * 10f);
        float visibleHeight = Mathf.Min(
            ContentRect.height,
            RowDisplayCount * (MenuTheme.ControlHeight + MenuTheme.ControlGap)
        );
        float contentHeight = keyValuePairs.Count * (MenuTheme.ControlHeight + MenuTheme.ControlGap);

        Rect listRect = new(x, 0f, ColumnWidth, visibleHeight);
        ReportContentBottom(listRect.yMax);

        scrollPosition = GUI.BeginScrollView(
            listRect,
            scrollPosition,
            new Rect(0f, 0f, ColumnWidth - 18f, contentHeight),
            false,
            contentHeight > listRect.height,
            GUIStyle.none,
            MenuTheme.VerticalScrollbarStyle,
            MenuTheme.ScrollViewBackgroundStyle
        );

        int i = 0;
        foreach (KeyValuePair<string, Tuple<bool, Action>> button in keyValuePairs)
        {
            Rect rect = new(
                0f,
                i * (MenuTheme.ControlHeight + MenuTheme.ControlGap),
                ColumnWidth - 18f,
                MenuTheme.ControlHeight
            );

            GUIStyle style = button.Value.Item1
                ? MenuTheme.ActiveButtonStyle
                : MenuTheme.ButtonStyle;

            if (GUI.Button(rect, button.Key, style))
            {
                button.Value.Item2?.Invoke();
            }

            i++;
        }

        GUI.EndScrollView();
    }

    public void AddSelectionList(
        IReadOnlyList<Tuple<string, bool, Action>> items,
        ref Vector2 scrollPosition,
        int rowDisplayCount = 8
    )
    {
        MenuTheme.EnsureInitialised();

        float rowHeight = MenuTheme.ControlHeight + MenuTheme.ControlGap;
        float visibleHeight = Mathf.Min(
            ContentRect.height - CurrentY,
            rowDisplayCount * rowHeight
        );
        visibleHeight = Mathf.Max(MenuTheme.ControlHeight, visibleHeight);

        float contentHeight = Mathf.Max(
            visibleHeight,
            items.Count * rowHeight
        );

        Rect listRect = new(CurrentX, CurrentY, ColumnWidth, visibleHeight);
        ReportContentBottom(listRect.yMax);

        scrollPosition = GUI.BeginScrollView(
            listRect,
            scrollPosition,
            new Rect(0f, 0f, ColumnWidth - 18f, contentHeight),
            false,
            contentHeight > listRect.height,
            GUIStyle.none,
            MenuTheme.VerticalScrollbarStyle,
            MenuTheme.ScrollViewBackgroundStyle
        );

        for (int i = 0; i < items.Count; i++)
        {
            Tuple<string, bool, Action> item = items[i];
            Rect rect = new(
                0f,
                i * rowHeight,
                ColumnWidth - 18f,
                MenuTheme.ControlHeight
            );

            GUIStyle style = item.Item2
                ? MenuTheme.ActiveButtonStyle
                : MenuTheme.ButtonStyle;

            if (GUI.Button(rect, item.Item1, style))
            {
                item.Item3?.Invoke();
            }
        }

        GUI.EndScrollView();
        ButtonIndex += Mathf.CeilToInt(visibleHeight / rowHeight);
    }

    public void AddImgListItem(
        Texture2D img,
        string itemname,
        UnityAction buttonAction,
        int rowLen
    )
    {
        Rect buttonRect = new(
            ContainterImgX * ContainterIndex_x,
            ContainterImgY * ContainterIndex_y,
            ContainterImgX,
            ContainterImgY
        );

        MenuTheme.DrawPanel(buttonRect, MenuTheme.Surface);
        GUI.Label(buttonRect, itemname, MenuTheme.MutedLabelStyle);

        if (GUI.Button(buttonRect, img, GUIStyle.none))
        {
            buttonAction();
        }

        ContainterIndex_x++;
        if (ContainterIndex_x >= rowLen)
        {
            ContainterIndex_y++;
            ContainterIndex_x = 0;
        }
    }

    public void AddGridItem(Texture2D img, string label, Action onClick)
    {
        const int spacing = 8;
        Rect buttonRect = new(
            ContainterIndex_x * (ContainterImgX + spacing),
            ContainterIndex_y * (ContainterImgY + spacing),
            ContainterImgX,
            ContainterImgY
        );

        MenuTheme.DrawPanel(buttonRect, MenuTheme.Surface);
        GUI.Label(buttonRect, label, MenuTheme.MutedLabelStyle);

        if (GUI.Button(buttonRect, img, GUIStyle.none))
        {
            onClick?.Invoke();
        }

        ContainterIndex_x++;
        if (ContainterIndex_x >= gridRowLen)
        {
            ContainterIndex_x = 0;
            ContainterIndex_y++;
        }
    }

    public void BeginIconGrid(
        ref Vector2 scrollPos,
        int rowLen,
        int totalItems,
        float viewHeight
    )
    {
        gridRowLen = rowLen;
        const int spacing = 8;

        float x = CurrentX;
        float y = CurrentY;
        int contentWidth = (ContainterImgX + spacing) * rowLen;
        int contentHeight = Mathf.CeilToInt((float)totalItems / rowLen)
            * (ContainterImgY + spacing);

        Rect gridRect = new(x, y, ColumnWidth, viewHeight);
        ReportContentBottom(gridRect.yMax);

        scrollPos = GUI.BeginScrollView(
            gridRect,
            scrollPos,
            new Rect(0f, 0f, contentWidth, contentHeight),
            false,
            contentHeight > gridRect.height,
            GUIStyle.none,
            MenuTheme.VerticalScrollbarStyle,
            MenuTheme.ScrollViewBackgroundStyle
        );

        ContainterIndex_x = 0;
        ContainterIndex_y = 0;
    }

    public void EndIconGrid()
    {
        GUI.EndScrollView();
        ButtonIndex += Mathf.CeilToInt(
            320f / (MenuTheme.ControlHeight + MenuTheme.ControlGap)
        );
    }

    private void AddButtonInternal(
        string label,
        UnityAction buttonAction,
        bool toggle
    )
    {
        MenuTheme.EnsureInitialised();

        Rect rect = GetControlRect();
        GUIStyle style = toggle
            ? MenuTheme.ActiveButtonStyle
            : MenuTheme.ButtonStyle;

        try
        {
            if (GUI.Button(rect, label, style))
            {
                buttonAction();
            }
        }
        catch (Exception exception)
        {
            Debug.LogError(exception);
        }

        ButtonIndex++;
    }

    private static float DrawSlider(
        Rect rect,
        float currentValue,
        float minValue,
        float maxValue,
        bool enabled
    )
    {
        int controlId = GUIUtility.GetControlID(FocusType.Passive, rect);
        Event currentEvent = Event.current;

        if (enabled)
        {
            switch (currentEvent.GetTypeForControl(controlId))
            {
                case EventType.MouseDown:
                    if (currentEvent.button == 0 && rect.Contains(currentEvent.mousePosition))
                    {
                        GUIUtility.hotControl = controlId;
                        currentValue = SliderValueFromMouse(rect, minValue, maxValue);
                        currentEvent.Use();
                    }
                    break;

                case EventType.MouseDrag:
                    if (GUIUtility.hotControl == controlId)
                    {
                        currentValue = SliderValueFromMouse(rect, minValue, maxValue);
                        currentEvent.Use();
                    }
                    break;

                case EventType.MouseUp:
                    if (GUIUtility.hotControl == controlId)
                    {
                        currentValue = SliderValueFromMouse(rect, minValue, maxValue);
                        GUIUtility.hotControl = 0;
                        currentEvent.Use();
                    }
                    break;
            }
        }

        float normalised = Mathf.InverseLerp(minValue, maxValue, currentValue);
        MenuTheme.DrawSlider(rect, normalised, enabled);
        return Mathf.Clamp(currentValue, minValue, maxValue);
    }

    private static float SliderValueFromMouse(
        Rect rect,
        float minValue,
        float maxValue
    )
    {
        float normalised = Mathf.InverseLerp(
            rect.x,
            rect.xMax,
            Event.current.mousePosition.x
        );

        return Mathf.Clamp(
            Mathf.Lerp(minValue, maxValue, normalised),
            minValue,
            maxValue
        );
    }

    private float DrawSliderInput(
        Rect rect,
        float currentValue,
        float minValue,
        float maxValue,
        float step,
        bool enabled
    )
    {
        string key = $"slider-{CurrentColumn}-{ButtonIndex}";
        string controlName = $"{GetInstanceID()}-{key}";
        bool focused = GUI.GetNameOfFocusedControl() == controlName;

        if (!sliderInputValues.ContainsKey(key) || !focused)
        {
            sliderInputValues[key] = FormatSliderValue(currentValue, step);
        }

        bool previousEnabled = GUI.enabled;
        GUI.enabled = enabled;

        GUI.SetNextControlName(controlName);
        string input = GUI.TextField(
            rect,
            sliderInputValues[key],
            10,
            MenuTheme.SliderInputStyle
        );
        sliderInputValues[key] = input;

        if (enabled && float.TryParse(input, out float parsedValue))
        {
            currentValue = SnapSliderValue(
                parsedValue,
                minValue,
                maxValue,
                step
            );
        }

        GUI.enabled = previousEnabled;

        if (enabled && focused && Event.current.type == EventType.KeyDown
            && (Event.current.keyCode == KeyCode.Return
                || Event.current.keyCode == KeyCode.KeypadEnter))
        {
            GUI.FocusControl(null);
            Event.current.Use();
        }

        return currentValue;
    }

    private static float SnapSliderValue(
        float value,
        float minValue,
        float maxValue,
        float step
    )
    {
        float clamped = Mathf.Clamp(value, minValue, maxValue);
        if (step <= 0f)
        {
            return clamped;
        }

        float snapped = minValue
            + (Mathf.Round((clamped - minValue) / step) * step);
        return Mathf.Clamp(snapped, minValue, maxValue);
    }

    private static string FormatSliderValue(float value, float step)
    {
        if (step <= 0f)
        {
            return value.ToString("0.###");
        }

        if (step >= 1f)
        {
            return Mathf.RoundToInt(value).ToString();
        }

        if (step >= 0.1f)
        {
            return value.ToString("F1");
        }

        return value.ToString("F2");
    }

    private Rect GetControlRect(float height = MenuTheme.ControlHeight)
    {
        if (ButtonIndex == 0)
        {
            columnVerticalOffsets[ColumnIndex] = 0f;
        }

        Rect rect = new(CurrentX, CurrentY, ColumnWidth, height);
        ReportContentBottom(rect.yMax);
        return rect;
    }

    private static void ReportContentBottom(float bottom)
    {
        CurrentPageContentHeight = Mathf.Max(
            CurrentPageContentHeight,
            bottom + MenuTheme.PageBottomPadding
        );
    }
}