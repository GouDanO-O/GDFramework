﻿using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.IO;
using System.Linq;

namespace QFramework
{
    public class ConsoleWindow : MonoBehaviour
    {
        /// <summary>
        /// Update回调
        /// </summary>
        public delegate void OnUpdateCallback();

        /// <summary>
        /// OnGUI回调
        /// </summary>
        public delegate void OnGUICallback();

        public OnUpdateCallback onUpdateCallback = null;

        public OnGUICallback onGUICallback = null;

        public bool ShowGUI
        {
            get => showGUI;
            set => showGUI = value;
        }

        private bool showGUI = true;

#if UNITY_IOS
        bool                 mTouching = false;
#endif

        private const int margin = 20;

        private Rect windowRect = new(margin + 960 * 0.5f, margin, 960 * 0.5f - 2 * margin,
            540 - 2 * margin);

        public bool OpenInAwake = false;

        private void Awake()
        {
            ConsoleKit.InitModules();
            showGUI = OpenInAwake;
            DontDestroyOnLoad(this);
        }

        private void OnDestroy()
        {
            ConsoleKit.DestroyModules();
        }

        private void Update()
        {
#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN
            if (Input.GetKeyUp(KeyCode.F1))
                showGUI = !showGUI;
#elif UNITY_ANDROID
            if (Input.GetKeyUp (KeyCode.Escape))
                this.showGUI = !this.showGUI;
#elif UNITY_IOS
            if (!mTouching && Input.touchCount == 4) {
                mTouching = true;
                this.showGUI = !this.showGUI;
            } else if (Input.touchCount == 0) {
                mTouching = false;
            }
#endif

            if (onUpdateCallback != null)
                onUpdateCallback();
        }

        private int mIndex = 0;

        private void OnGUI()
        {
            if (!showGUI)
                return;

            if (onGUICallback != null)
                onGUICallback();

            var cachedMatrix = GUI.matrix;
            IMGUIHelper.SetDesignResolution(960, 540);

            if (GUI.Button(new Rect(100, 100, 100, 50), "清空数据"))
            {
                PlayerPrefs.DeleteAll();

                Directory.Delete(Application.persistentDataPath, true);
#if UNITY_EDITOR
                EditorApplication.isPlaying = false;
#else
                Application.Quit ();
#endif
            }

            windowRect = GUILayout.Window(int.MaxValue / 2, windowRect, DrawConsoleWindow, "控制台");
            GUI.matrix = cachedMatrix;
        }


        /// <summary>
        /// A window displaying the logged messages.
        /// </summary>
        private void DrawConsoleWindow(int windowID)
        {
            mIndex = GUILayout.Toolbar(mIndex, ConsoleKit.Modules.Select(m => m.Title).ToArray());
            ConsoleKit.Modules[mIndex].DrawGUI();
            GUI.DragWindow(new Rect(0, 0, 10000, 20));
        }
    }
}