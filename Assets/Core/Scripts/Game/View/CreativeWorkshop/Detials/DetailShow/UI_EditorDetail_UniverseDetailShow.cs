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
    public class UI_EditorDetail_UniverseDetailShow : UI_EditorDetail_DetailShow<UniverseDtoDef>
    {
        /// <summary>
        /// 更新当前的宇宙详情显示
        /// </summary>
        /// <param name="universeDef"></param>
        public override void UpdateDetailShow(UniverseDtoDef universeDef)
        {
            base.UpdateDetailShow(universeDef);
        }
        
    }
}