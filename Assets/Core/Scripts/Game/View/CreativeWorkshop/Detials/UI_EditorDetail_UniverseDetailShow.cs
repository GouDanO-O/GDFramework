using System.Collections.Generic;
using Core.Game.Chunk.Universe.Data;
using GDFrameworkExtend.UIKit;
using TMPro;
using UnityEngine.UI;

namespace Core.Game.View.Details
{
    public class UI_EditorDetail_UniverseDetailShow : UI_Details
    {
        protected TextMeshProUGUI UniverseIdText;
        
        protected TMP_InputFieldExtended UniverseNameInput;
        
        protected TMP_InputFieldExtended UniverseDescInput;
        

        protected override void OnInit()
        {
            UniverseIdText = transform.Find("UniverseIdText/Text").GetComponent<TextMeshProUGUI>();
            UniverseNameInput = transform.Find("UniverseNameInput").GetComponent<TMP_InputFieldExtended>();
            UniverseDescInput = transform.Find("UniverseDescInput").GetComponent<TMP_InputFieldExtended>();
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