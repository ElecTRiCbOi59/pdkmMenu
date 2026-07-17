using pdkmMenu;
using Unity.Netcode;
using UnityEngine;


public class CompanyMenu : MonoBehaviour
{
    private guiBase companyMenuGUI;

    private void Start()
    {
        companyMenuGUI = gameObject.AddComponent<guiBase>();
        companyMenuGUI.MenuColor = companyMenuGUI.CustomBlue;
        companyMenuGUI.YPercentage = 0.0f;
    }


    private static float companyBuyingRate = 0f;
    public void update()
    {
        companyMenuGUI.CurrentColumn = 1;
        companyMenuGUI.ButtonIndex = 0; 

        if (StartOfRound.Instance == null ) return;
        companyMenuGUI.AddButton(LKey.AttackPlayers, () =>
        {
            DepositItemsDesk_Patches.depositItemsDesk?.AttackPlayersServerRpc();
        });
        companyMenuGUI.AddButton(LKey.SetPatience, () =>
        {
            DepositItemsDesk_Patches.depositItemsDesk?.SetPatienceServerRpc(-3);
        });
        
        //Causes the items to breaks
        //companyMenuGUI.AddButton("Add all Items to desk", () =>
        //{
        //    DepositItemsDesk desk = FindObjectOfType<DepositItemsDesk>();
        //    if (desk != null)
        //    {
        //        foreach (GrabbableObject item in FindObjectsOfType<GrabbableObject>())
        //        {
        //            if (item.GetComponent<NetworkObject>() != null)
        //            {
        //                desk.AddObjectToDeskServerRpc(item.gameObject.GetComponent<NetworkObject>());
        //            }
        //        }
        //    }
        //});

        if (StartOfRound.Instance.IsServer)
        {
            if (DepositItemsDesk_Patches.depositItemsDesk != null)
            {
                companyMenuGUI.AddButton(LKey.ToggleDoor, () => {
                    DepositItemsDesk_Patches.depositItemsDesk.OpenShutDoorClientRpc(!Object.FindObjectOfType<DepositItemsDesk>().doorOpen);
                }, DepositItemsDesk_Patches.depositItemsDesk.doorOpen);
                companyMenuGUI.AddButton(LKey.MakeNoise, () => {
                    DepositItemsDesk_Patches.depositItemsDesk.MakeWarningNoiseClientRpc();});
            }

            if (companyBuyingRate == 0)
            {
                companyBuyingRate = StartOfRound.Instance.companyBuyingRate;
            }
            companyBuyingRate = companyMenuGUI.AddSlider(0.1f, 10, companyBuyingRate, companyBuyingRate.ToString(), StartOfRound.Instance == null);
            companyMenuGUI.AddButton(LKey.SetBuyRate, () =>
            {
                StartOfRound.Instance.companyBuyingRate = companyBuyingRate;
                StartOfRound.Instance.SyncCompanyBuyingRateServerRpc();
            }, StartOfRound.Instance == null);
        }
        companyMenuGUI.CurrentColumn = 0;
        companyMenuGUI.ButtonIndex = 0;
    }
}

