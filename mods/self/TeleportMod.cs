using UnityEngine;

namespace pdkmMenu
{
    public class TeleportMod
    {
        public static Vector3 RecallPos = Vector3.zero;
        public static Vector3 MarkedPos = Vector3.zero;
        public static void TeleportToEntrance()
        {
            StartOfRound.Instance.localPlayerController.isInsideFactory = true;
            TeleportToPos(RoundManager.FindMainEntrancePosition(true, false));
        }
        public static void TeleportToShip()
        {
            StartOfRound.Instance.localPlayerController.isInsideFactory = false;
            TeleportToPos(StartOfRound.Instance.shipBounds.bounds.center);
        }
        public static void MarkPos()
        {
            MarkedPos = StartOfRound.Instance.localPlayerController.transform.position;
        }
        public static void TeleportToMarkPos()
        {
            if (MarkedPos == Vector3.zero) return;
            TeleportToPos(MarkedPos);
        }

        public static void Recall()
        {
            if(RecallPos == Vector3.zero)
            {
                RecallPos = StartOfRound.Instance.localPlayerController.transform.position;
            }
            Vector3 NewPos = RecallPos;
            RecallPos = StartOfRound.Instance.localPlayerController.transform.position;
            StartOfRound.Instance.localPlayerController.transform.position = NewPos;
        }
        
        public static void TeleportToPos(Vector3 pos)
        {
            RecallPos = StartOfRound.Instance.localPlayerController.transform.position;
            StartOfRound.Instance.localPlayerController.transform.position = pos;
        }
    }
}
