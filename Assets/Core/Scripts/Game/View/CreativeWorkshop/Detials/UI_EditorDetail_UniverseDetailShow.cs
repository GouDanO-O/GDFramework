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
    public class UI_EditorDetail_UniverseDetailShow : UI_EditorDetail_DetailShow
    {


        private UniverseDtoDef _curUniverseDtoDef;
        
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



        protected override void DefNameInput_OnValueChanged(string newValue)
        {
            base.DefNameInput_OnValueChanged(newValue);
        }
        
        protected override void DefDescInput_OnValueChanged(string newValue)
        {
            base.DefDescInput_OnValueChanged(newValue);
        }

    }
}