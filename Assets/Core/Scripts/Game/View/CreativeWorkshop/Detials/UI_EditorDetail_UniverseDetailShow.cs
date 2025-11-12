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
        
        protected TextMeshProUGUI UniverseIdText;
        
        protected TMP_InputField UniverseNameInput;
        
        protected TMP_InputField UniverseDescInput;

        private UniverseDtoDef _curUniverseDtoDef;
        
        protected override void OnInit()
        {
            ContentRoot = transform.GetComponent<ScrollRect>().content;
            UniverseIdText = ContentRoot.Find("UniverseIdText/Text").GetComponent<TextMeshProUGUI>();
            UniverseNameInput = ContentRoot.Find("UniverseNameInput").GetComponent<TMP_InputField>();
            UniverseDescInput = ContentRoot.Find("UniverseDescInput").GetComponent<TMP_InputField>();
            
            UniverseNameInput.onValueChanged.AddListener(UniverseNameInput_OnValueChanged);
            UniverseDescInput.onValueChanged.AddListener(UniverseDescInput_OnValueChanged);
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
            UniverseIdText.text = universeDef.DefId;
            UniverseNameInput.text = universeDef.DefName;
            UniverseDescInput.text = universeDef.DefDescription;
        }

        private void UniverseNameInput_OnValueChanged(string newValue) 
        {
            
        }

        private void UniverseDescInput_OnValueChanged(string newValue) 
        {
            
        }

        public string GetUniverseName()
        {
            return UniverseNameInput.text;
        }

        public string GetUniverseDesc()
        {
            return UniverseDescInput.text;
        }

    }
}