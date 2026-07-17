using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace pdkmMenu
{
    public class ToolTipsMod
    {
        public static void UpdateMod()
        {
            if (Plugin.SelfSettings.ToolTips.Value)
            {
                return;
            }
            if(HUDManager.Instance == null) return;
            HUDManager.Instance.ClearControlTips();
        }
    }
}
