using QFramework;
using UnityEngine;
using UnityEngine.UI;

namespace GDFramework_Core.Scripts.GDFrameworkExtend._CoreKit.ActionKit.Scripts.ScreenTransition
{
    [MonoSingletonPath("QFramework/ActionKit/ScreenTransitionCanvas")]
    internal class ScreenTransitionCanvas : MonoBehaviour, ISingleton
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