using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pdkmMenu
{
    public class GodModeMod
    {
        public static void UpdateMod()
        {
            //allow local player death this is inverted
            StartOfRound.Instance.allowLocalPlayerDeath = !Plugin.SelfSettings.GodMode.Value;
        }
        public static void ToggleGodMode()
        {
            Plugin.SelfSettings.GodMode.Value = !Plugin.SelfSettings.GodMode.Value;
            LogToChat.LogLocal($"GodMode: {Plugin.SelfSettings.GodMode.Value}");
        }
    }
}
