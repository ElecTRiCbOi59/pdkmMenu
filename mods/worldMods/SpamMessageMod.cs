using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace pdkmMenu
{
    public class SpamMessageMod
    {
        private static float timer = 0f;
        private static float rotationValue = 0f;

        // Adjust these to change the speed and frequency
        private static WorldConfig worldConfig = Plugin.WorldSettings;
        public static void Update()
        {
            if (!worldConfig.SpamMessage.Value) return;
            timer += Time.deltaTime;

            if (timer >= 0.2f)
            {
                timer = 0f;

                // Increment rotation (0 to 360)
                rotationValue = (rotationValue + worldConfig.SpamMessageRotation.Value) % 360;

                // Properly format the string with the current rotation
                // We use string interpolation ($"") to insert the rotationValue

                string payload = $"<indent=150%><rotate={rotationValue}><size=500><sprite index={worldConfig.SpamMessageIndex.Value}></size>";

                // Send to the server
                LogToChat.SendServerMessage(payload);

            }
        }
    }
}
