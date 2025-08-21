using UnityEngine;
using GDFrameworkCore;
using GDFrameworkExtend.PoolKit;
using GDFrameworkExtend.StorageKit;
using Sirenix.OdinInspector;
using UnityEngine.Events;

namespace Game.World
{
    public class Node : MonoBehaviour,ICanGetSystem,IPoolable
    {
        public bool IsRecycled { get; set; }
        
        [ShowInInspector]
        protected NodeData CurNodeData;
        
        protected NodePointChecker NodePointChecker;
        
        public UnityAction<Vector2> OnDragNodeEvent;
        
        public UnityAction OnClickNodeEvent;
        
        protected RectTransform RectTransform;

        public IArchitecture GetArchitecture()
        {
            return Main.Interface;
        }

        private void Start()
        {
            InitNode();
        }

        public void InitNode()
        {
            this.CurNodeData = new NodeData();
            this.CurNodeData.InitNodeData(this);
            
            this.InitData();
            this.RegisterEvents();

            this.CurNodeData.ChangeTempPosition(RectTransform.anchoredPosition);
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
            
            RectTransform=this.GetComponent<RectTransform>();
        }

        private void RegisterEvents()
        {
            this.OnDragNodeEvent += OnDragEventHandle;
        }
        
        private void UnregisterEvents()
        {
            this.OnDragNodeEvent -= OnDragEventHandle;
        }

        public void SetNodeData(NodeData nodeData)
        {

        }

        private void UpdateVisualState(ENodeState state)
        {
            switch (state)
            {
                case ENodeState.Hidden:
                    HideThisNode();
                    break;
                case ENodeState.Locked:
                    ShowThisNode();
                    // 可以添加锁定状态的视觉效果
                    break;
                case ENodeState.Triggerable:
                    ShowThisNode();
                    break;
                case ENodeState.Triggered:

                    break;
            }
        }

        public void OnRecycled()
        {
            UnregisterEvents();
            CurNodeData?.DestroyNodeData();
            CurNodeData = null;
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

        private void OnDragEventHandle(Vector2 point)
        {
            CurNodeData.ChangeTempPosition(point);
        }
        
        /// <summary>
        /// 触发节点交互
        /// </summary>
        public void TriggerInteraction()
        {
            if (CheckCondition())
            {
                OnClickNodeEvent?.Invoke();
            }
        }
    }
}
