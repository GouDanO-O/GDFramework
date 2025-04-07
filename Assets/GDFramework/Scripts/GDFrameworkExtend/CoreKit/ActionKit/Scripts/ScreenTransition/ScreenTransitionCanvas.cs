using UnityEngine;
using UnityEngine.UI;

namespace GDFrameworkExtend.CoreKit
{
    [MonoSingletonPath("GDFrameworkExtend.CoreKit/ActionKit/ScreenTransitionCanvas")]
    internal class ScreenTransitionCanvas :MonoBehaviour, ISingleton
    {
        internal static ScreenTransitionCanvas Instance =>
            PrefabSingletonProperty<ScreenTransitionCanvas>.InstanceWithLoader(
                Resources.Load<GameObject>);
        
        public Image ColorImage;
        
        public void OnSingletonInit()
        {
            
        }
    }
}
