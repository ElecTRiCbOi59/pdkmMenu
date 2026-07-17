using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using pdkmMenu;

public class guiBase : MonoBehaviour
{
    public Color CustomRed = new Color(0.5f, 0.2f, 0.2f, 1.0f);
    public Color CustomBlue = new Color(0.2f, 0.3f, 0.8f, 1.0f);
    public Color MenuColor;
    public Color TextColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);

    public float ButtonWidthPercentage = 0.06f;
    public float ButtonHeightPercentage = 0.025f;
    public float PaddingYPercentage = 0.03f;

    public float XPercentage = 0.0f;
    public float YPercentage = 0.0f;
    public int ButtonIndex;

    public int CurrentColumn = 0;
    public float ColumnWidthOffset = 0.07f; // Percentage of screen width to shift per column

    private float GetCurrentX() => Screen.width * (XPercentage + (CurrentColumn * ColumnWidthOffset));

    public float infoWidthPercentage = 0.06f;
    public float infoHeighthPercentage = 0.015f;
    public int InfoIndex;

    public int ContainerX;
    public int ContainterY;
    public int ContainterIndex_x;
    public int ContainterIndex_y;
    public int ContainterImgX = 75;
    public int ContainterImgY = 75;
    private GUIStyle buttonStyle;
    private const int ButtonFontMax = 13;
    private const int ButtonFontMin = 8;
    private const float ButtonTextPadding = 6f;

    private void EnsureButtonStyle()
    {
        if (buttonStyle != null) return;
        buttonStyle = new GUIStyle(GUI.skin.button)
        {
            alignment = TextAnchor.MiddleCenter,
            wordWrap = false,
            clipping = TextClipping.Clip
        };
    }

    private int GetFittedFontSize(string text, float maxWidth)
    {
        if (string.IsNullOrEmpty(text)) return ButtonFontMax;
        EnsureButtonStyle();

        int size = ButtonFontMax;
        var content = new GUIContent(text);
        while (size > ButtonFontMin)
        {
            buttonStyle.fontSize = size;
            float width = buttonStyle.CalcSize(content).x;
            if (width <= maxWidth - ButtonTextPadding) break;
            size--;
        }
        return size;
    }

    public float AddSlider(float minValue, float maxValue, float currentValue, string label = "", bool toggle = true)
    {
        Color OriginalColor = MenuColor;
        if (!toggle) MenuColor = CustomRed;

        GUI.backgroundColor = MenuColor;
        GUI.contentColor = TextColor;

        float buttonWidth = Screen.width * ButtonWidthPercentage;
        float buttonHeight = Screen.height * ButtonHeightPercentage;
        float buttonX = GetCurrentX();
        float buttonY = Screen.height * (YPercentage + ButtonIndex * PaddingYPercentage);

        Rect buttonRect = new Rect(buttonX, buttonY, buttonWidth, buttonHeight);
        Rect labelRect = new Rect(buttonX, buttonY + buttonHeight / 3, buttonWidth, buttonHeight - buttonHeight / 3);

        GUI.DrawTexture(buttonRect, Texture2D.whiteTexture, ScaleMode.StretchToFill, true, 0, MenuColor, 0, 0);
        float result = GUI.HorizontalSlider(buttonRect, currentValue, minValue, maxValue);
        GUI.Label(labelRect, label);

        MenuColor = OriginalColor;
        ButtonIndex++;
        return result;
    }

    public int AddSlider(int minValue, int maxValue, int currentValue, string label = "", bool toggle = true)
    {
        return Mathf.RoundToInt(AddSlider((float)minValue, (float)maxValue, (float)currentValue, label, toggle));
    }

    // Localized Version (Uses Enum)
    public void AddButton(LKey key, UnityAction buttonAction, bool toggle = true)
    {
        AddButtonInternal(Localization.T(key), buttonAction, toggle);
    }

    // Raw String Version (Overrides for untranslated text)
    public void AddButton(string rawName, UnityAction buttonAction, bool toggle = true)
    {
        AddButtonInternal(rawName, buttonAction, toggle);
    }

    // Private helper to prevent code duplication
    private void AddButtonInternal(string label, UnityAction buttonAction, bool toggle)
    {
        Color OriginalColor = MenuColor;
        if (!toggle) MenuColor = CustomRed;

        GUI.backgroundColor = MenuColor;
        GUI.contentColor = TextColor;

        float buttonWidth = Screen.width * ButtonWidthPercentage;
        float buttonHeight = Screen.height * ButtonHeightPercentage;
        float buttonX = GetCurrentX();
        float buttonY = Screen.height * (YPercentage + ButtonIndex * PaddingYPercentage);

        Rect buttonRect = new Rect(buttonX, buttonY, buttonWidth, buttonHeight);
        GUI.DrawTexture(buttonRect, Texture2D.whiteTexture, ScaleMode.StretchToFill, true, 0, MenuColor, 0, 0);

        try
        {
            EnsureButtonStyle();
            buttonStyle.fontSize = GetFittedFontSize(label, buttonRect.width);
            if (GUI.Button(buttonRect, label, buttonStyle)) buttonAction();
        }
        catch (Exception e) { Debug.LogError(e); }

        MenuColor = OriginalColor;
        ButtonIndex++;
    }

    public string TextBox(string currentValue)
    {
        GUI.backgroundColor = MenuColor;
        GUI.contentColor = TextColor;

        float buttonWidth = Screen.width * ButtonWidthPercentage;
        float buttonHeight = Screen.height * ButtonHeightPercentage;
        float buttonX = GetCurrentX();
        float buttonY = Screen.height * (YPercentage + ButtonIndex * PaddingYPercentage);

        Rect textRect = new Rect(buttonX, buttonY, buttonWidth, buttonHeight);
        GUI.DrawTexture(textRect, Texture2D.whiteTexture, ScaleMode.StretchToFill, true, 0, MenuColor, 0, 0);
        currentValue = GUI.TextField(textRect, currentValue, 25);

        ButtonIndex++;
        return currentValue;
    }

    public int IntTextBox(int currentValue)
    {
        string input = TextBox(currentValue.ToString());
        return int.TryParse(input, out int result) ? result : currentValue;
    }

    public void AddButtonList(Dictionary<string, Tuple<bool, Action>> keyValuePairs, ref Vector2 scrollPosition, int index = 0, int RowDisplayCount = 10)
    {
        // Custom column logic for scroll views
        int xOffset = (int)(GetCurrentX() + (index * (Screen.width * 0.015f)));
        int visibleHeight = (int)(Screen.height * PaddingYPercentage) * RowDisplayCount;
        int contentHeight = (int)(Screen.height * (keyValuePairs.Count * PaddingYPercentage));

        scrollPosition = GUI.BeginScrollView(
            new Rect(xOffset, Screen.height * YPercentage, (Screen.width * ButtonWidthPercentage) + (Screen.width * 0.01f), visibleHeight),
            scrollPosition,
            new Rect(0, 0, (Screen.width * ButtonWidthPercentage), contentHeight));

        int i = 0;
        foreach (var button in keyValuePairs)
        {
            Color originalColor = MenuColor;
            MenuColor = button.Value.Item1 ? CustomBlue : CustomRed;
            Rect buttonRect = new Rect(0, Screen.height * (i * PaddingYPercentage), Screen.width * ButtonWidthPercentage, Screen.height * ButtonHeightPercentage);

            GUI.DrawTexture(buttonRect, Texture2D.whiteTexture, ScaleMode.StretchToFill, true, 0, MenuColor, 0, 0);
            GUI.backgroundColor = MenuColor;
            string localized = button.Key;//Localization.T(button.Key);
            EnsureButtonStyle();
            buttonStyle.fontSize = GetFittedFontSize(localized, buttonRect.width);
            if (GUI.Button(buttonRect, localized, buttonStyle)) button.Value.Item2.Invoke();

            MenuColor = originalColor;
            GUI.backgroundColor = originalColor;
            i++;
        }
        GUI.EndScrollView();
    }

    public void AddImgListItem(Texture2D img, string itemname, UnityAction buttonAction, int rowLen)
    {
        // Remove ContainerX and ContainerY here. 
        // The ScrollView handles the global positioning.
        Rect buttonRect = new Rect(
            (float)(ContainterImgX * ContainterIndex_x),
            (float)(ContainterImgY * ContainterIndex_y),
            ContainterImgX, ContainterImgY);

        GUI.DrawTexture(buttonRect, Texture2D.whiteTexture, ScaleMode.StretchToFill, true, 0, MenuColor, 0, 0);
        GUI.Label(buttonRect, itemname);
        if (GUI.Button(buttonRect, img)) buttonAction();

        ContainterIndex_x++;
        if (ContainterIndex_x >= rowLen) // Changed from > to >= to match rowLen exactly
        {
            ContainterIndex_y++;
            ContainterIndex_x = 0;
        }
    }
    private int gridRowLen;

    public void AddGridItem(Texture2D img, string label, Action onClick)
    {
        // 1. Spacing logic (Adjust 5 to whatever gap you want)
        int spacing = 5;
        Rect buttonRect = new Rect(
            ContainterIndex_x * (ContainterImgX + spacing),
            ContainterIndex_y * (ContainterImgY + spacing),
            ContainterImgX,
            ContainterImgY
        );

        // 2. Match your original style exactly
        GUI.backgroundColor = MenuColor;
        GUI.contentColor = TextColor;

        // 3. Draw exactly like your original AddImgListItem
        GUI.DrawTexture(buttonRect, Texture2D.whiteTexture, ScaleMode.StretchToFill, true, 0, MenuColor, 0, 0);
        GUI.Label(buttonRect, label);
        if (GUI.Button(buttonRect, img)) onClick?.Invoke();

        // 4. Wrap to next row
        ContainterIndex_x++;
        if (ContainterIndex_x >= gridRowLen)
        {
            ContainterIndex_x = 0;
            ContainterIndex_y++;
        }
    }

    public void BeginIconGrid(ref Vector2 scrollPos, int rowLen, int totalItems, float viewHeight)
    {
        this.gridRowLen = rowLen;
        int spacing = 5;

        float x = GetCurrentX();
        float y = Screen.height * (YPercentage + ButtonIndex * PaddingYPercentage);

        // Width = (IconSize + Gap) * Number of Icons + Scrollbar padding
        int windowWidth = ((ContainterImgX + spacing) * rowLen) + 20;
        int contentWidth = (ContainterImgX + spacing) * rowLen;
        int contentHeight = (Mathf.CeilToInt((float)totalItems / rowLen)) * (ContainterImgY + spacing);

        scrollPos = GUI.BeginScrollView(
            new Rect(x, y, windowWidth, viewHeight),
            scrollPos,
            new Rect(0, 0, contentWidth, contentHeight)
        );

        ContainterIndex_x = 0;
        ContainterIndex_y = 0;
    }

    public void EndIconGrid()
    {
        GUI.EndScrollView();
        // Advance the button index so the next UI element doesn't overlap the grid
        ButtonIndex += Mathf.CeilToInt((Screen.height * 0.4f) / (Screen.height * PaddingYPercentage));
    }
}
