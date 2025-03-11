using QFramework;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Frame.Models
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

