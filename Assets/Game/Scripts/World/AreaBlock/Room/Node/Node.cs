// 1. 完善 NodeComponent.cs 的缺失部分
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;
using GDFrameworkExtend.PoolKit;
using UnityEngine.Events;
using UnityEngine.Pool;

namespace Game.World
{
    public class Node : MonoBehaviour,IPoolable
    {
        public bool IsRecycled { get; set; }
        
        protected NodeData CurNodeData;
        
        protected NodePointChecker NodePointChecker;
        
        public UnityAction<Vector2> OnDragNodeEvent;
        
        public UnityAction OnClickNodeEvent;
        
        public void InitNode()
        {
            this.InitData();
            this.RegisterEvents();
        }
        
        public void InitNode(NodeData nodeData)
        {
            CurNodeData=nodeData;
            this.InitData();
            this.RegisterEvents();
            this.SetNodeData(nodeData);
        }

        private void InitData()
        {
            if (NodePointChecker == null)
            {
                NodePointChecker = this.gameObject.AddComponent<NodePointChecker>();
                NodePointChecker.InitNodePointChecker(this);
            }
        }

        private void RegisterEvents()
        {
            
        }

        public void SetNodeData(NodeData nodeData)
        {
            
        }

        public void OnRecycled()
        {
            
        }

        /// <summary>
        /// 显示节点
        /// </summary>
        public void ShowThisNode()
        {
            
        }

        /// <summary>
        /// 隐藏节点
        /// </summary>
        public void HideThisNode()
        {
            
        }

        /// <summary>
        /// 能否进行互动
        /// </summary>
        public bool CanInteract()
        {
            return CurNodeData.CanTrigger();
        }

        /// <summary>
        /// 检查是否有条件
        /// </summary>
        /// <returns></returns>
        public bool CheckCondition()
        {
            return CurNodeData.CheckCondition();
        }
        
        /// <summary>
        /// 能否进行移动
        /// </summary>
        /// <returns></returns>
        public bool CanMoveable()
        {
            return CurNodeData.CanMoveable();
        }
        
        
    }
}
