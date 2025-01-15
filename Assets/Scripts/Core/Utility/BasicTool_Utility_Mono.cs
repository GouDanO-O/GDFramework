using System;
using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;

namespace Game
{
    public abstract class BasicTool_Utility_Mono : MonoBehaviour,IUtility
    {
        [HideInInspector]public bool isShowing;
        
        // Start is called before the first frame update
        private void Awake()
        {
            InitUtility();
        }

        private void OnDestroy()
        {
            DeInitUtility();
        }

        protected abstract void InitUtility();

        private void OnGUI()
        {
            DrawGUI();
        }

        protected virtual void DrawGUI()
        {
            
        }

        public virtual void CheckButtonWillShow()
        {
            isShowing = !isShowing;
        }
        
        protected virtual void DeInitUtility()
        {
            
        }
    }
}

