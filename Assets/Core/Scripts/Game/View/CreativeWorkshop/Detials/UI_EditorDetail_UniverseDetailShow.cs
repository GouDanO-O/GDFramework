using System.Collections.Generic;
using Core.Game.Chunk.Universe.Data;
using GDFrameworkExtend.UIKit;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Game.View.Details
{
    /// <summary>
    /// 宇宙数据详情编辑器
    /// </summary>
    public class UI_EditorDetail_UniverseDetailShow : UI_Details
    {
        protected Transform ContentRoot;
        
        protected TextMeshProUGUI DefIdText;
        
        protected TMP_InputField DefNameInput;
        
        protected TMP_InputField DefDescInput;

        private UniverseDtoDef _curUniverseDtoDef;
        
        protected override void OnInit()
        {
            ContentRoot = transform.GetComponent<ScrollRect>().content;
            DefIdText = ContentRoot.Find("DefIdText/Text").GetComponent<TextMeshProUGUI>();
            DefNameInput = ContentRoot.Find("DefNameInput").GetComponent<TMP_InputField>();
            DefDescInput = ContentRoot.Find("DefDescInput").GetComponent<TMP_InputField>();
            
            DefNameInput.onValueChanged.AddListener(UniverseNameInput_OnValueChanged);
            DefDescInput.onValueChanged.AddListener(UniverseDescInput_OnValueChanged);
        }

        protected override void OnShow()
        {
            
        }

        protected override void OnStart()
        {
            
        }

        protected override void OnClose()
        {
            
        }

        /// <summary>
        /// 更新当前的宇宙详情显示
        /// </summary>
        /// <param name="universeDef"></param>
        public void UpdateDetailShow(UniverseDtoDef universeDef)
        {
            _curUniverseDtoDef = universeDef;
            DefIdText.text = universeDef.DefId;
            DefNameInput.text = universeDef.DefName;
            DefDescInput.text = universeDef.DefDescription;
        }

        private void UniverseNameInput_OnValueChanged(string newValue) 
        {
            
        }

        private void UniverseDescInput_OnValueChanged(string newValue) 
        {
            
        }

        public string GetUniverseName()
        {
            return DefNameInput.text;
        }

        public string GetUniverseDesc()
        {
            return DefDescInput.text;
        }

    }
}