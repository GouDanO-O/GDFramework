using System.Collections;
using System.Collections.Generic;
using GDFrameworkExtend.UIKit;
using UnityEngine;

namespace GDFrameworkExtend
{
    public class ResKitUIPanelTester : MonoBehaviour
    {
 
            /// <summary>
            /// 页面的名字
            /// </summary>
            public string PanelName;

            /// <summary>
            /// 层级名字
            /// </summary>
            public UILevel Level;

            [SerializeField] private List<UIPanelTesterInfo> mOtherPanels;

            private void Awake()
            {
                GDFrameworkExtend.ResKit.ResKit.Init();
            }

            private IEnumerator Start()
            {
                yield return new WaitForSeconds(0.2f);
			
                GDFrameworkExtend.UIKit.UIKit.OpenPanel(PanelName, Level);

                mOtherPanels.ForEach(panelTesterInfo => { GDFrameworkExtend.UIKit.UIKit.OpenPanel(panelTesterInfo.PanelName, panelTesterInfo.Level); });
            }
    }
}