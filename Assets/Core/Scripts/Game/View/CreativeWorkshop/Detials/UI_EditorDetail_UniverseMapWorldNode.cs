using Core.Game.Chunk.World.Data;
using UnityEngine;
using UnityEngine.EventSystems;
using Core.Game.View.Details;
using GDFrameworkExtend.FluentAPI;
using GDFrameworkExtend.UIKit;
using TMPro;
using UnityEngine.UI;

namespace Core.Game.View.Details
{
    /// <summary>
    /// 星图编辑器中的世界节点
    /// </summary>
    public class UI_EditorDetail_UniverseMapWorldNode : UI_EditorDetail_MapNode
    {

        
        private WorldDtoDef _worldDto;
        
        
        /// <summary>
        /// 展示当前的世界详情
        /// </summary>
        private void ShowWorldDetail()
        {
            UIKit.GetPanel<UI_Editor_UniversePanel>().OpenWorldDetail(_worldDto);
        }
        
        //TODO 复制当前世界
        /// <summary>
        /// 复制当前世界
        /// </summary>
        private void CopyThisWorld()
        {
            
        }

        /// <summary>
        /// 改变当前世界是否为初始世界
        /// </summary>
        /// <param name="isInitialWorld"></param>
        public override void ChangeInitialNode(bool isInitialNode)
        {
            base.ChangeInitialNode(isInitialNode);

        }
        
        
    }
}