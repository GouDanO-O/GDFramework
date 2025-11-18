using Core.Game.Chunk.Universe.Data;
using GDFrameworkExtend.UIKit;
using TMPro;
using UnityEngine.UI;

namespace Core.Game.View.Details
{
    /// <summary>
    /// 宇宙编辑器中的宇宙列表中的列表单项
    /// </summary>
    public class UI_EditorDetail_UniverseListItem : UI_Details
    {
        private UniverseDtoDef _universeDef;
        
        private TextMeshProUGUI _universeName;
        
        private TextMeshProUGUI _universeDescription;

        private TextMeshProUGUI _ownedWorldCount;
        
        private Button _showDetailsButton;
        
        private Button _copyButton;
        
        private Button _deleteThisButton;
        
        protected override void OnInit()
        {
            _universeName = transform.Find("UniverseName/Name").GetComponent<TextMeshProUGUI>();
            _universeDescription = transform.Find("UniverseDescription/Description").GetComponent<TextMeshProUGUI>();
            _ownedWorldCount = transform.Find("OwnedWorldCount/WorldCount").GetComponent<TextMeshProUGUI>();
            
            _showDetailsButton = transform.Find("ShowDetailsButton").GetComponent<Button>();
            _copyButton = transform.Find("CopyButton").GetComponent<Button>();
            _deleteThisButton = transform.Find("DeleteThisButton").GetComponent<Button>();
            
            
            _showDetailsButton.onClick.AddListener(ShowUniverseDetail);
            _copyButton.onClick.AddListener(CopyThisUniverseDetail);
            _deleteThisButton.onClick.AddListener(DeleteUniverseDetail);
        }

        protected override void OnShow()
        {
            UpdateUniverseData();
        }

        protected override void OnStart()
        {
            
        }

        protected override void OnClose()
        {
            
        }

        /// <summary>
        /// 当前列表项绑定的宇宙固定数据
        /// </summary>
        /// <param name="universeDef"></param>
        public void SetThisUniverseData(UniverseDtoDef universeDef)
        {
            _universeDef = universeDef;
        }

        /// <summary>
        /// 更新宇宙数据
        /// </summary>
        public void UpdateUniverseData()
        {
            if(_universeDef == null)
                return;

            _universeName.text = _universeDef.DefName;
            _universeDescription.text = _universeDef.DefDescription;
            _ownedWorldCount.text = _universeDef.WorldIdList.Count.ToString();
        }

        /// <summary>
        /// 展示宇宙详情
        /// </summary>
        private void ShowUniverseDetail()
        {
            UIKit.GetPanel<UI_Editor_UniversePanel>().SelectUniverse(_universeDef);
        }

        /// <summary>
        /// 复制当前宇宙
        /// </summary>
        private void CopyThisUniverseDetail()
        {
            
        }

        /// <summary>
        /// 删除宇宙
        /// </summary>
        private void DeleteUniverseDetail()
        {
            
        }
    }
}