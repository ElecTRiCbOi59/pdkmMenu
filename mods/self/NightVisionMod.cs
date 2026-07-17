using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace pdkmMenu
{
    public class NightVisionMod
    {

        public static GameObject LightHolder = null;
        public static Light light = null;
        public static void UpdateMod()
        {
            if(LightHolder == null)
            {
                CreateLight();
                if (LightHolder == null) return;
            }

            UpdatePos();
            if (!Plugin.SelfSettings.NightVision.Value && !FreeCamMod.IsFreeCam)
            {
                LightHolder.SetActive(false);
                return;
            }



           




            LightHolder.SetActive(true);
            light.intensity = Plugin.SelfSettings.NightVisionIntensity.Value;
        }
        private static void UpdatePos()
        {
            if (FreeCamMod.IsFreeCam)
            {
                if(FreeCamMod.FreeCamera == null) return ;
                light.transform.localEulerAngles = new Vector3(
              FreeCamMod.FreeCamera.transform.localEulerAngles.x + 90,
              FreeCamMod.FreeCamera.transform.localEulerAngles.y, 0);
                return;
            }
            light.transform.localEulerAngles = new Vector3(
              GameNetworkManager.Instance.localPlayerController.transform.localEulerAngles.x + 90,
              GameNetworkManager.Instance.localPlayerController.transform.localEulerAngles.y, 0);
        }

        private static void CreateLight()
        {
            LightHolder = new GameObject("lightermfer");
            LightHolder.hideFlags = HideFlags.HideAndDontSave;
            UnityEngine.Object.DontDestroyOnLoad(LightHolder);
            light = LightHolder.AddComponent<Light>();
            light.type = LightType.Directional;
            LightHolder.AddComponent<HDAdditionalLightData>();
            light.gameObject.GetComponent<HDAdditionalLightData>().intensity = 10f;
            //light.gameObject.GetComponent<HDAdditionalLightData>().EnableShadows(false);
            light.intensity = 50f;
            LightHolder.SetActive(false);
            Plugin.Logger.LogInfo("Light Created");
        }

    }
}
