using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game
{
    public class ResourcesData_Model : AbstractModel
    {
        public InputActionAsset InputActionAsset { get; set; }
        
        public TextAsset Multilingual { get; set; }
        
        protected override void OnInit()
        {
            
        }
    }
}

