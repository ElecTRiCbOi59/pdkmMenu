using pdkmMenu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CamerasMenu : MonoBehaviour
{
    private guiBase camerasMenuGUI;
    private void Start()
    {
        camerasMenuGUI = gameObject.AddComponent<guiBase>();
        camerasMenuGUI.MenuColor = camerasMenuGUI.CustomBlue;
        camerasMenuGUI.XPercentage = 0.07f;
        camerasMenuGUI.YPercentage = 0.0f;
        //selfConfig = Plugin.SelfSettings;
    }
    public void update()
    {
        camerasMenuGUI.AddButton(LKey.ToggleCams, ()=> Spectate.ToggleSpectate(), Spectate.canvasObj.activeSelf);
        foreach (Camera camera in Camera.allCameras)
        {
            camerasMenuGUI.AddButton($"{camera.name}", () => { Spectate.SetViewCamera(camera); }, Spectate.targetCamera == camera);
        }
        camerasMenuGUI.ButtonIndex = 0; // Reset index
    }
}