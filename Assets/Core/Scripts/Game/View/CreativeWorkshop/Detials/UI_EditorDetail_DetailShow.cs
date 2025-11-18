using Core.Game.Chunk.Universe.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Game.View.Details
{
    public abstract class UI_EditorDetail_DetailShow: UI_Details
    {
        protected Transform ContentRoot;
        
        protected TextMeshProUGUI DefIdText;
        
        protected TMP_InputField DefNameInput;
        
        protected TMP_InputField DefDescInput;
        
        protected override void OnInit()
        {
            ContentRoot = transform.GetComponent<ScrollRect>().content;
            DefIdText = ContentRoot.Find("DefIdText/Text").GetComponent<TextMeshProUGUI>();
            DefNameInput = ContentRoot.Find("DefNameInput").GetComponent<TMP_InputField>();
            DefDescInput = ContentRoot.Find("DefDescInput").GetComponent<TMP_InputField>();
            
            DefNameInput.onValueChanged.AddListener(DefNameInput_OnValueChanged);
            DefDescInput.onValueChanged.AddListener(DefDescInput_OnValueChanged);
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
        public virtual void UpdateDetailShow(UniverseDtoDef universeDef)
        {
            DefIdText.text = universeDef.DefId;
            DefNameInput.text = universeDef.DefName;
            DefDescInput.text = universeDef.DefDescription;
        }

        protected virtual void DefNameInput_OnValueChanged(string newValue) 
        {
            
        }

        protected virtual void DefDescInput_OnValueChanged(string newValue) 
        {
            
        }

        public string GetDefName()
        {
            return DefNameInput.text;
        }

        public string GetDefDesc()
        {
            return DefDescInput.text;
        }
    }
}