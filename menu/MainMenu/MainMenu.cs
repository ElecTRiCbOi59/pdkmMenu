using pdkmMenu;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum MainMenuPages
{
    None,
    Self,
    Players,
    Cameras,
    World,
    Host,
    Company
}

internal class MainMenu : MonoBehaviour
{
    private const int WindowId = 731059;
    private static MainMenu instance;

    private SelfMenu selfMenu;
    private PlayersMenu playersMenu;
    private CamerasMenu camerasMenu;
    private WorldMenu worldMenu;
    private HostMenu hostMenu;
    private CompanyMenu companyMenu;

    private Rect windowRect;
    private readonly Vector2[] pageScrollPositions = new Vector2[7];
    private readonly float[] pageContentHeights = new float[7];
    private StartOfRound observedRound;
    private bool isQuickHidden;

    public MainMenuPages currentSubmenu = MainMenuPages.Self;
    public bool IsOpened;

    private void Awake()
    {
        instance = this;
    }

    private void OnEnable()
    {
        SceneManager.activeSceneChanged += HandleActiveSceneChanged;
    }

    private void OnDisable()
    {
        SceneManager.activeSceneChanged -= HandleActiveSceneChanged;
        CloseMenu();

        if (instance == this)
        {
            instance = null;
        }
    }

    private void Start()
    {
        selfMenu = gameObject.AddComponent<SelfMenu>();
        playersMenu = gameObject.AddComponent<PlayersMenu>();
        camerasMenu = gameObject.AddComponent<CamerasMenu>();
        worldMenu = gameObject.AddComponent<WorldMenu>();
        hostMenu = gameObject.AddComponent<HostMenu>();
        companyMenu = gameObject.AddComponent<CompanyMenu>();

        float width = Mathf.Min(MenuTheme.WindowWidth, Screen.width - 40f);
        float height = Mathf.Min(MenuTheme.WindowHeight, Screen.height - 40f);

        windowRect = new Rect(
            (Screen.width - width) / 2f,
            (Screen.height - height) / 2f,
            width,
            height
        );
    }

    private void Update()
    {
        CheckSessionLifecycle();
        CheckKeys();
    }

    private void CheckSessionLifecycle()
    {
        StartOfRound currentRound = StartOfRound.Instance;

        if (observedRound != null && currentRound != observedRound)
        {
            CloseMenu();
        }

        observedRound = currentRound;
    }

    private void HandleActiveSceneChanged(Scene previousScene, Scene nextScene)
    {
        CloseMenu();
        observedRound = StartOfRound.Instance;
    }

    private void CloseMenu()
    {
        IsOpened = false;
        isQuickHidden = false;
        MenuConfig.SetCursorVisible(false);
    }

    private void OnGUI()
    {
        if (!IsOpened)
        {
            return;
        }

        MenuTheme.EnsureInitialised();

        if (isQuickHidden)
        {
            DrawQuickRestoreButton();
            return;
        }

        windowRect = GUI.Window(WindowId, windowRect, DrawWindow, GUIContent.none, MenuTheme.WindowStyle);
    }

    private void DrawWindow(int windowId)
    {
        DrawHeader();
        DrawSidebar();
        DrawFooter();

        Rect pageViewport = new(
            MenuTheme.SidebarWidth + MenuTheme.OuterPadding,
            MenuTheme.HeaderHeight + MenuTheme.OuterPadding,
            windowRect.width - MenuTheme.SidebarWidth - (MenuTheme.OuterPadding * 2f),
            windowRect.height - MenuTheme.HeaderHeight - MenuTheme.FooterHeight - (MenuTheme.OuterPadding * 2f)
        );

        int pageIndex = (int)currentSubmenu;
        float contentHeight = Mathf.Max(
            pageViewport.height,
            pageContentHeights[pageIndex]
        );
        float contentWidth = Mathf.Max(0f, pageViewport.width - 18f);

        HandlePageScroll(pageViewport, pageIndex, contentHeight);

        pageScrollPositions[pageIndex] = GUI.BeginScrollView(
            pageViewport,
            pageScrollPositions[pageIndex],
            new Rect(0f, 0f, contentWidth, contentHeight),
            false,
            contentHeight > pageViewport.height,
            GUIStyle.none,
            MenuTheme.VerticalScrollbarStyle,
            MenuTheme.ScrollViewBackgroundStyle
        );

        guiBase.ContentRect = new Rect(0f, 0f, contentWidth, contentHeight);
        guiBase.ResetPageMeasurement(pageViewport.height);
        DrawCurrentPage();
        pageContentHeights[pageIndex] = guiBase.CurrentPageContentHeight;

        GUI.EndScrollView();

        GUI.DragWindow(new Rect(0f, 0f, windowRect.width, MenuTheme.HeaderHeight));
    }

    public static void ScrollPageToTop(MainMenuPages page)
    {
        if (instance == null)
        {
            return;
        }

        int pageIndex = (int)page;
        instance.pageScrollPositions[pageIndex] = Vector2.zero;
        instance.pageContentHeights[pageIndex] = 0f;
    }

    private void HandlePageScroll(
        Rect pageViewport,
        int pageIndex,
        float contentHeight
    )
    {
        Event currentEvent = Event.current;

        if (
            currentEvent.type != EventType.ScrollWheel
            || !pageViewport.Contains(currentEvent.mousePosition)
        )
        {
            return;
        }

        float maximumScroll = Mathf.Max(0f, contentHeight - pageViewport.height);

        if (maximumScroll <= 0f)
        {
            return;
        }

        Vector2 scrollPosition = pageScrollPositions[pageIndex];
        scrollPosition.y = Mathf.Clamp(
            scrollPosition.y + (currentEvent.delta.y * 32f),
            0f,
            maximumScroll
        );

        pageScrollPositions[pageIndex] = scrollPosition;
        currentEvent.Use();
    }

    private void DrawHeader()
    {
        GUI.Label(
            new Rect(24f, 11f, 440f, 34f),
            "PDKM Cheat Mod Menu",
            MenuTheme.HeaderTitleStyle
        );

        GUI.Label(
            new Rect(25f, 43f, 440f, 20f),
            $"Lethal Company  •  Mod v{MyPluginInfo.PLUGIN_VERSION}",
            MenuTheme.HeaderSubtitleStyle
        );

        DrawRoleStatus();

        MenuTheme.DrawPanel(
            new Rect(0f, MenuTheme.HeaderHeight - 1f, windowRect.width, 1f),
            MenuTheme.Border
        );
    }

    private void DrawQuickHideButton(float footerY)
    {
        const float buttonWidth = 118f;
        const float buttonHeight = 34f;

        Rect buttonRect = new(
            14f,
            footerY + 9f,
            buttonWidth,
            buttonHeight
        );

        GUIContent content = new("◀  Collapse", "Temporarily hide the menu");

        if (GUI.Button(buttonRect, content, MenuTheme.ActiveButtonStyle))
        {
            isQuickHidden = true;
        }
    }

    private void DrawQuickRestoreButton()
    {
        const float buttonWidth = 100f;
        const float buttonHeight = 40f;

        Rect buttonRect = new(
            4f,
            4f,
            buttonWidth,
            buttonHeight
        );

        GUIContent content = new("▶  Menu", "Restore the PDKM menu");

        if (GUI.Button(buttonRect, content, MenuTheme.ActiveButtonStyle))
        {
            isQuickHidden = false;
        }
    }

    private void DrawRoleStatus()
    {
        bool isHost = StartOfRound.Instance != null && StartOfRound.Instance.IsServer;
        bool isCoHost = !isHost
            && CoHostHandler.Instance != null
            && CoHostHandler.Instance.IsCoHost;

        string roleLabel;
        GUIStyle roleStyle;

        if (isHost)
        {
            roleLabel = "HOST";
            roleStyle = MenuTheme.HostStatusStyle;
        }
        else if (isCoHost)
        {
            roleLabel = "CO-HOST";
            roleStyle = MenuTheme.CoHostStatusStyle;
        }
        else
        {
            roleLabel = "CLIENT";
            roleStyle = MenuTheme.ClientStatusStyle;
        }

        const float pillWidth = 104f;
        GUI.Label(
            new Rect(windowRect.width - pillWidth - 24f, 20f, pillWidth, 32f),
            roleLabel,
            roleStyle
        );
    }

    private void DrawSidebar()
    {
        Rect sidebarRect = new(
            0f,
            MenuTheme.HeaderHeight,
            MenuTheme.SidebarWidth,
            windowRect.height - MenuTheme.HeaderHeight - MenuTheme.FooterHeight
        );

        MenuTheme.DrawPanel(sidebarRect, MenuTheme.SidebarBackground);

        float x = 14f;
        float y = MenuTheme.HeaderHeight + 18f;
        float width = MenuTheme.SidebarWidth - 28f;
        const float height = 44f;
        const float gap = 9f;

        DrawNavigationButton(GetSelfNavigationLabel(), MainMenuPages.Self, x, ref y, width, height, gap);
        DrawNavigationButton(LKey.Players, MainMenuPages.Players, x, ref y, width, height, gap);
        DrawNavigationButton(LKey.Cameras, MainMenuPages.Cameras, x, ref y, width, height, gap);
        DrawNavigationButton(LKey.World, MainMenuPages.World, x, ref y, width, height, gap);

        if (CanUseHostMenu())
        {
            DrawNavigationButton(
                CoHostHandler.Instance != null && CoHostHandler.Instance.IsCoHost
                    ? LKey.CoHost
                    : LKey.Host,
                MainMenuPages.Host,
                x,
                ref y,
                width,
                height,
                gap
            );
        }

        DrawNavigationButton(LKey.Company, MainMenuPages.Company, x, ref y, width, height, gap);
    }

    private void DrawNavigationButton(
        LKey key,
        MainMenuPages page,
        float x,
        ref float y,
        float width,
        float height,
        float gap
    )
    {
        DrawNavigationButton(
            Capitalise(Localization.T(key)),
            page,
            x,
            ref y,
            width,
            height,
            gap
        );
    }

    private void DrawNavigationButton(
        string label,
        MainMenuPages page,
        float x,
        ref float y,
        float width,
        float height,
        float gap
    )
    {
        bool selected = currentSubmenu == page;
        GUIStyle style = selected
            ? MenuTheme.NavButtonSelectedStyle
            : MenuTheme.NavButtonStyle;
        Rect buttonRect = new(x, y, width, height);

        if (GUI.Button(buttonRect, label, style))
        {
            SetCurrentPage(page);
        }

        y += height + gap;
    }

    private static string GetSelfNavigationLabel()
    {
        string selfLabel = Capitalise(Localization.T(LKey.Self));
        string playerName = StartOfRound.Instance?.localPlayerController?.playerUsername;

        return string.IsNullOrWhiteSpace(playerName)
            ? selfLabel
            : $"{selfLabel} ({playerName})";
    }

    private static string Capitalise(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return value;
        }

        return char.ToUpperInvariant(value[0]) + value.Substring(1);
    }

    private void DrawFooter()
    {
        float y = windowRect.height - MenuTheme.FooterHeight;

        MenuTheme.DrawPanel(
            new Rect(0f, y, windowRect.width, MenuTheme.FooterHeight),
            MenuTheme.SidebarBackground
        );
        MenuTheme.DrawPanel(new Rect(0f, y, windowRect.width, 1f), MenuTheme.Border);

        string languageLabel = string.Format(
            Localization.T(LKey.LanguageFormat),
            Localization.CurrentLanguage
        );

        DrawQuickHideButton(y);

        const float reloadButtonWidth = 160f;
        const float languageButtonWidth = 210f;
        const float rightPadding = 14f;
        const float gap = 10f;

        float reloadButtonX = windowRect.width - rightPadding - reloadButtonWidth;
        float languageButtonX = reloadButtonX - gap - languageButtonWidth;

        if (GUI.Button(
            new Rect(languageButtonX, y + 9f, languageButtonWidth, 34f),
            languageLabel,
            MenuTheme.FooterButtonStyle
        ))
        {
            Localization.CycleLanguage();
        }

        if (GUI.Button(
            new Rect(reloadButtonX, y + 9f, reloadButtonWidth, 34f),
            Localization.T(LKey.ReloadConfig),
            MenuTheme.FooterButtonStyle
        ))
        {
            Plugin.configFile.Reload();
        }
    }

    private void DrawCurrentPage()
    {
        switch (currentSubmenu)
        {
            case MainMenuPages.Self:
                selfMenu.update();
                break;

            case MainMenuPages.Players:
                playersMenu.update();
                break;

            case MainMenuPages.Cameras:
                camerasMenu.update();
                break;

            case MainMenuPages.World:
                worldMenu.update();
                break;

            case MainMenuPages.Host:
                hostMenu.update();
                break;

            case MainMenuPages.Company:
                companyMenu.update();
                break;
        }
    }

    private void CheckKeys()
    {
        if (Plugin.MenuSettings.OpenMenu.Value.IsDown())
        {
            if (IsOpened)
            {
                CloseMenu();
            }
            else
            {
                IsOpened = true;
                isQuickHidden = false;
            }
        }

        if (Plugin.MenuSettings.OpenSelfMenu.Value.IsDown())
        {
            OpenPage(MainMenuPages.Self);
        }

        if (Plugin.MenuSettings.OpenPlayersMenu.Value.IsDown())
        {
            OpenPage(MainMenuPages.Players);
        }

        if (Plugin.MenuSettings.OpenCamerasMenu.Value.IsDown())
        {
            OpenPage(MainMenuPages.Cameras);
        }

        if (Plugin.MenuSettings.OpenworldMenu.Value.IsDown())
        {
            OpenPage(MainMenuPages.World);
        }

        if (Plugin.MenuSettings.OpenhostMenu.Value.IsDown())
        {
            OpenPage(MainMenuPages.Host);
        }
    }

    private void OpenPage(MainMenuPages page)
    {
        SetCurrentPage(page);
        IsOpened = true;
        isQuickHidden = false;
    }

    private void SetCurrentPage(MainMenuPages page)
    {
        if (currentSubmenu == MainMenuPages.Host && page != MainMenuPages.Host)
        {
            hostMenu?.ResetItemSpawner();
        }

        currentSubmenu = page;
    }

    private static bool CanUseHostMenu()
    {
        bool isServer = StartOfRound.Instance != null && StartOfRound.Instance.IsServer;
        bool isCoHost = CoHostHandler.Instance != null && CoHostHandler.Instance.IsCoHost;
        return isServer || isCoHost;
    }
}