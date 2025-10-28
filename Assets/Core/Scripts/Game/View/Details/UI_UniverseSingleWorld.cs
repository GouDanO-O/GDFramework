using Core.Game.Chunk.World.Data;
using GDFrameworkExtend.UIKit;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Game.View.Details
{
    public class UI_UniverseSingleWorld : UI_Details
    {
        private UI_UniversePanel _uiUniversePanel;

        protected Button ClickButton;

        protected WorldData BindingWorldData;

        private RectTransform _rectTransform;

        private TextMeshProUGUI _worldName;
        
        public void SetWorldData(WorldData worldData)
        {
            BindingWorldData = worldData;
        }
        
        protected override void OnInit()
        {
            _uiUniversePanel = UIKit.GetPanel<UI_UniversePanel>();
            ClickButton = this.GetComponent<Button>();
            _rectTransform = this.GetComponent<RectTransform>();
            _worldName = this.transform.Find("WorldName").GetComponent<TextMeshProUGUI>();
        }

        protected override void OnShow()
        {
            UpdateWorldPosition();
        }

        protected override void OnStart()
        {
            ClickButton.onClick.AddListener(ClickButtonCheck);
        }

        protected override void OnClose()
        {
            
        }

        /// <summary>
        /// 更新世界坐标
        /// </summary>
        protected void UpdateWorldPosition()
        {
            if (_rectTransform)
            {
                if (BindingWorldData != null)
                {
                    _rectTransform.anchoredPosition = BindingWorldData.WorldDef.InitialSpawnedPosition;
                    _worldName.text = BindingWorldData.WorldDef.DefName;
                }
            }
        }
        
        /// <summary>
        /// 当点击了星图中的这个世界
        /// </summary>
        protected void ClickButtonCheck()
        {
            if (BindingWorldData != null && _uiUniversePanel)
            {
                _uiUniversePanel.UpdateCurSelectingWorld(BindingWorldData);
            }
        }
    }
}

