using BepInEx.Configuration;
using pdkmMenu;
using UnityEngine;


public enum MainMenuPages
{
    None,
    Self,
    Players,
    Cameras,
    world,
    host,
    company,
    lobby,
    cohost
}
internal class MainMenu : MonoBehaviour
{
    private guiBase mainMenu;

    private SelfMenu selfMenu;
    private PlayersMenu playersMenu;
    private CamerasMenu camerasMenu;
    private WorldMenu worldMenu;
    private HostMenu hostMenu;
    private CompanyMenu companyMenu;

    public MainMenuPages currentSubmenu = MainMenuPages.None;



    private void Start()
    {
        


        // Create an instance of the CustomMenuGUI component
        mainMenu = gameObject.AddComponent<guiBase>();
        selfMenu = gameObject.AddComponent<SelfMenu>();
        playersMenu = gameObject.AddComponent<PlayersMenu>();
        camerasMenu = gameObject.AddComponent<CamerasMenu>();
        worldMenu = gameObject.AddComponent<WorldMenu>();
        hostMenu = gameObject.AddComponent<HostMenu>();
        companyMenu = gameObject.AddComponent<CompanyMenu>();


        // Set up the menu colors
        mainMenu.MenuColor = mainMenu.CustomBlue;
        //mainMenu.SetTextColor(new Vector4(1.0f, 1.0f, 1.0f, 1.0f)); // White text
        mainMenu.XPercentage = 0.0f;
        mainMenu.YPercentage = 0.0f;

    }
    public bool IsOpened = false;
    private void CheckKeys()
    {
        if (Plugin.MenuSettings.OpenMenu.Value.IsDown())
        {
            IsOpened = !IsOpened;
        }

        if (Plugin.MenuSettings.OpenSelfMenu.Value.IsDown())
        {
            SetSubMenu(MainMenuPages.Self);
        }
        if (Plugin.MenuSettings.OpenPlayersMenu.Value.IsDown())
        {
            SetSubMenu(MainMenuPages.Players);
        }
        if (Plugin.MenuSettings.OpenCamerasMenu.Value.IsDown())
        {
            SetSubMenu(MainMenuPages.Cameras);
        }
        if (Plugin.MenuSettings.OpenworldMenu.Value.IsDown())
        {
            SetSubMenu(MainMenuPages.world);
        }
        if (Plugin.MenuSettings.OpenhostMenu.Value.IsDown())
        {
            SetSubMenu(MainMenuPages.host);
        }
    }
    private void Update()
    {
        CheckKeys();
    }
    private void OnGUI()
    {
        //Plugin.Logger.LogInfo("balls");

        if (!IsOpened) return;

        // Render the GUI
        mainMenu.AddButton(LKey.ReloadConfig, OnButtonClicked);

        mainMenu.AddButton(LKey.Self, () => SetSubMenu(MainMenuPages.Self));
        mainMenu.AddButton(LKey.Players, () => SetSubMenu(MainMenuPages.Players));
        mainMenu.AddButton(LKey.Cameras, () => SetSubMenu(MainMenuPages.Cameras));
        mainMenu.AddButton(LKey.World, () => SetSubMenu(MainMenuPages.world));
        if (StartOfRound.Instance != null)
        {
            if (StartOfRound.Instance.IsServer)
            {
                mainMenu.AddButton(LKey.Host, () => SetSubMenu(MainMenuPages.host));
            }
        }
        if (CoHostHandler.Instance != null)
        {
          if(CoHostHandler.Instance.IsCoHost) 
            {
                mainMenu.AddButton(LKey.CoHost, () => SetSubMenu(MainMenuPages.host));
            }
        }
        mainMenu.AddButton(LKey.Company, () => SetSubMenu(MainMenuPages.company));
        string langLabel = string.Format(Localization.T(LKey.LanguageFormat), Localization.CurrentLanguage);
        mainMenu.AddButton(langLabel, () => Localization.CycleLanguage());

        //mainMenu.AddButton("self", () => { showSelf = !showSelf; });

        mainMenu.ButtonIndex = 0; // Reset ButtonIndex to handle dynamic positioning


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
            case MainMenuPages.world:
                worldMenu.update();
                break;
            case MainMenuPages.host:
                hostMenu.update();
                break;
            case MainMenuPages.company:
                companyMenu.update();
                break;
            case MainMenuPages.None:
                break;
        }
    }

    private void OnButtonClicked()
    {
        Plugin.configFile.Reload();
    }
    public void SetSubMenu(MainMenuPages input)
    {
        if (input == currentSubmenu)
        {
            currentSubmenu = MainMenuPages.None;
        }
        else
        {
            currentSubmenu = input;
        }
    }

}
