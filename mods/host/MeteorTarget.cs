using GameNetcodeStuff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace pdkmMenu
{
    public class MeteorTarget
    {
        //never tested 
        public static void TargetPlayerWithPrediction(PlayerControllerB targetPlayer)
        {
            // 1. Calculate Lead Time
            // How long (in real seconds) does it take for 1.0 of normalized time to pass?
            float totalDayTime = TimeOfDay.Instance.totalTime;
            float landTimeNormalized = TimeOfDay.Instance.normalizedTimeOfDay + 0.02f;
            float timeToImpact = 0.02f * totalDayTime;

            // 2. Predict Position
            // Position = CurrentPosition + (Velocity * Time)
            Vector3 playerVelocity = targetPlayer.thisController.velocity;

            // We only predict X and Z to avoid the meteor trying to land in the air 
            // if the player jumps.
            Vector3 predictedPos = targetPlayer.transform.position + (playerVelocity * timeToImpact);

            // 3. Ground the predicted position (Raycast down to find floor)
            RaycastHit hit;
            if (Physics.Raycast(predictedPos + Vector3.up * 20f, Vector3.down, out hit, 50f, StartOfRound.Instance.collidersAndRoomMaskAndDefault))
            {
                predictedPos = hit.point;
            }

            // 4. Spawn using the original RPC logic
            float size = 15f;
            Vector3 skyDirection = Vector3.up * 10f;

            Meteor predictedMeteor = new Meteor();
            predictedMeteor.normalizedLandingTime = landTimeNormalized;
            predictedMeteor.scale = size;
            predictedMeteor.skyDirection = skyDirection;
            predictedMeteor.landingPosition = predictedPos;

            TimeOfDay.Instance.MeteorWeather.meteors.Add(predictedMeteor);
            TimeOfDay.Instance.MeteorWeather.meteorsEnabled = true;

            TimeOfDay.Instance.MeteorWeather.CreateMeteorServerRpc(landTimeNormalized, predictedPos, skyDirection, size);

        }
    }
}
