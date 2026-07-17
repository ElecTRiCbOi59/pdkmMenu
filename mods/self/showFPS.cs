using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace pdkmMenu
{
    internal class showFPS
    {
        private float deltaTime = 0.0f;
        private GUIStyle style;
        private Rect rect;

        void Start()
        {
            style = new GUIStyle();
            style.alignment = TextAnchor.UpperRight;
            style.fontSize = 20;
            style.normal.textColor = Color.white;

            rect = new Rect(Screen.width - 150, 10, 140, 30);
        }

        void Update()
        {
            deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        }

        void OnGUI()
        {
            if (style == null) return;

            float fps = 1.0f / deltaTime;
            string text = $"FPS: {fps:0.}";

            rect.x = Screen.width - 150; // keep top-right if resolution changes
            GUI.Label(rect, text, style);
        }
    }
}
