using System;
using UnityEngine;

namespace Core.Game.View.Details
{
    public abstract class UI_Details : MonoBehaviour
    {
        private void Awake()
        {
            OnInit();
        }

        private void OnEnable()
        {
            OnShow();
        }
        
        private void Start()
        {
            OnStart();
        }

        private void OnDisable()
        {
            OnClose();
        }

        protected abstract void OnInit();

        protected abstract void OnShow();

        protected abstract void OnStart();

        protected abstract void OnClose();
    }
}

