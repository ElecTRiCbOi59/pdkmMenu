using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pdkmMenu
{
    public class InfTetra
    {
        public static void UpdateMod()
        {
            if (!Plugin.SelfSettings.InfTet.Value) return;
            if (GameNetworkManager.Instance.localPlayerController == null) return;
            if (GameNetworkManager.Instance.localPlayerController.ItemSlots == null) return;

            GrabbableObject[] slots = GameNetworkManager.Instance.localPlayerController.ItemSlots;
            foreach (GrabbableObject item in slots)
            {
                if (item == null) continue;
                if (item is not TetraChemicalItem) continue;
                ReflectionUtil.ReflectSetField<TetraChemicalItem>(item, "fuel", 1f);
            }
        }

    }
}