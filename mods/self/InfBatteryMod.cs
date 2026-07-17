using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pdkmMenu
{
    public class InfBatteryMod
    {
        public static void UpdateMod()
        {
            if (!Plugin.SelfSettings.InfBattery.Value) return;
            if (GameNetworkManager.Instance.localPlayerController == null) return;
            if (GameNetworkManager.Instance.localPlayerController.ItemSlots == null) return;

            GrabbableObject[] slots = GameNetworkManager.Instance.localPlayerController.ItemSlots;
            foreach (GrabbableObject item in slots)
            {
                if (item == null) continue;
                if (item.insertedBattery == null) continue;

                item.insertedBattery.empty = false;
                item.insertedBattery.charge = 100f;
            }
        }

    }
}