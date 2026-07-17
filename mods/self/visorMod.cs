using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


namespace pdkmMenu
{
    public class VisorMod
    {
        private static GameObject Visor = null;
        public static void UpdateMod()
        {
            if(Visor == null)
            {
                Visor = GameObject.Find("Systems/Rendering/PlayerHUDHelmetModel/");
                return;
            }
            Visor.SetActive(Plugin.SelfSettings.Visor.Value);
        }
    }
}
