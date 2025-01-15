using System;
using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;

namespace Game
{
    public class InputManager_Old : MonoSingleton<InputManager_Old>,IController
    {
        public IArchitecture GetArchitecture()
        {
            throw new System.NotImplementedException();
        }

        private void Update()
        {
            CheckKeyCode();
        }

        private void CheckKeyCode()
        {
            
        }
    }
}

