using System.Collections.Generic;
using Core.Game.Chunk.Universe.Data;
using GDFrameworkExtend.UIKit;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Game.View.Details
{
    public class UI_EditorDetail_UniverseDetailShow : UI_Details
    {
        protected Transform ContentRoot;
        
        protected TextMeshProUGUI UniverseIdText;
        
        protected TMP_InputField UniverseNameInput;
        
        protected TMP_InputField UniverseDescInput;
        

        protected override void OnInit()
        {
            ContentRoot = transform.GetComponent<ScrollRect>().content;
            UniverseIdText = ContentRoot.Find("UniverseIdText/Text").GetComponent<TextMeshProUGUI>();
            UniverseNameInput = ContentRoot.Find("UniverseNameInput").GetComponent<TMP_InputField>();
            UniverseDescInput = ContentRoot.Find("UniverseDescInput").GetComponent<TMP_InputField>();
            
            
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
            UniverseIdText.text = universeDef.DefId;
            UniverseNameInput.text = universeDef.DefName;
            UniverseDescInput.text = universeDef.DefDescription;
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