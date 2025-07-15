// 1. 完善 NodeComponent.cs 的缺失部分
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;

namespace Game.World
{
    public class NodeComponent : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("UI 组件引用")]
        [SerializeField] private Button nodeButton;
        [SerializeField] private Image nodeImage;
        [SerializeField] private TextMeshProUGUI nodeNameText;
        [SerializeField] private Image nodeStateIndicator;
        [SerializeField] private GameObject nodeTooltip;
        [SerializeField] private CanvasGroup canvasGroup;
        
        [Header("节点状态颜色")]
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color triggerableColor = Color.green;
        [SerializeField] private Color triggeredColor = Color.gray;
        [SerializeField] private Color lockedColor = Color.red;
        
        [Header("动画设置")]
        [SerializeField] private float hoverScale = 1.1f;
        [SerializeField] private float hoverDuration = 0.2f;
        [SerializeField] private AnimationCurve hoverCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        
        // 节点数据
        private NodeData nodeData;
        private int depth = 0;
        private bool isVisible = false;
        private bool isInteractable = true;
        
        // 状态
        private NodeState currentState = NodeState.Normal;
        
        // 动画相关
        private Coroutine hoverAnimation;
        private Vector3 originalScale;
        
        // 事件
        public event Action<NodeComponent> OnNodeClicked;
        public event Action<NodeComponent> OnNodeHovered;
        public event Action<NodeComponent> OnNodeUnhovered;
        
        private void Awake()
        {
            InitializeComponents();
            originalScale = transform.localScale;
        }
        
        private void Start()
        {
            InitNodeComponent();
        }
        
        /// <summary>
        /// 初始化组件引用
        /// </summary>
        private void InitializeComponents()
        {
            // 自动获取组件引用（如果没有手动设置）
            if (nodeButton == null)
                nodeButton = GetComponent<Button>();
            
            if (nodeImage == null)
                nodeImage = GetComponent<Image>();
            
            if (nodeNameText == null)
                nodeNameText = GetComponentInChildren<TextMeshProUGUI>();
            
            if (canvasGroup == null)
                canvasGroup = GetComponent<CanvasGroup>();
            
            // 如果没有CanvasGroup，添加一个
            if (canvasGroup == null)
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            
            // 设置默认状态
            if (nodeImage != null)
                nodeImage.color = normalColor;
        }
        
        /// <summary>
        /// 初始化节点组件
        /// </summary>
        public void InitNodeComponent()
        {
            if (nodeData != null)
            {
                UpdateNodeDisplay();
                UpdateNodeState();
            }
        }
        
        #region 数据设置和获取
        
        /// <summary>
        /// 设置节点数据
        /// </summary>
        public void SetNodeData(NodeData data)
        {
            nodeData = data;
            UpdateNodeDisplay();
            UpdateNodeState();
        }
        
        /// <summary>
        /// 获取节点数据
        /// </summary>
        public NodeData GetNodeData()
        {
            return nodeData;
        }
        
        /// <summary>
        /// 设置节点深度
        /// </summary>
        public void SetDepth(int nodeDepth)
        {
            depth = nodeDepth;
        }
        
        /// <summary>
        /// 获取节点深度
        /// </summary>
        public int GetDepth()
        {
            return depth;
        }
        
        #endregion
        
        #region 显示和状态更新
        
        /// <summary>
        /// 更新节点显示
        /// </summary>
        private void UpdateNodeDisplay()
        {
            if (nodeData?.NodeDataPersistent == null) return;
            
            // 更新节点名称
            if (nodeNameText != null)
            {
                nodeNameText.text = nodeData.NodeDataPersistent.nodeName;
            }
            
            // 更新按钮状态
            if (nodeButton != null)
            {
                nodeButton.interactable = isInteractable;
            }
        }
        
        /// <summary>
        /// 更新节点状态
        /// </summary>
        public void UpdateNodeState()
        {
            if (nodeData?.NodeDataTemporary == null) return;
            
            NodeState newState = DetermineNodeState();
            if (newState != currentState)
            {
                currentState = newState;
                ApplyStateVisuals();
            }
        }
        
        /// <summary>
        /// 确定节点状态
        /// </summary>
        private NodeState DetermineNodeState()
        {
            if (nodeData.NodeDataTemporary.hasBeanTrigger)
            {
                return NodeState.Triggered;
            }
            
            // 检查是否可以触发（简化版本，实际应该调用NodeManager的检查方法）
            if (CanBTriggered())
            {
                return NodeState.Triggerable;
            }
            
            if (!nodeData.NodeDataTemporary.hasBeanShowing)
            {
                return NodeState.Hidden;
            }
            
            return NodeState.Locked;
        }
        
        /// <summary>
        /// 简化的触发条件检查
        /// </summary>
        private bool CanBTriggered()
        {
            // 如果已经触发过，不能再次触发
            if (nodeData.NodeDataTemporary.hasBeanTrigger)
                return false;
            
            // 如果还没有显示，不能触发
            if (!nodeData.NodeDataTemporary.hasBeanShowing)
                return false;
            
            // 这里应该有更复杂的条件检查逻辑
            // 实际项目中应该通过NodeManager来检查
            return true;
        }
        
        /// <summary>
        /// 应用状态视觉效果
        /// </summary>
        private void ApplyStateVisuals()
        {
            if (nodeImage == null) return;
            
            Color targetColor = normalColor;
            bool interactable = true;
            
            switch (currentState)
            {
                case NodeState.Normal:
                    targetColor = normalColor;
                    break;
                case NodeState.Triggerable:
                    targetColor = triggerableColor;
                    break;
                case NodeState.Triggered:
                    targetColor = triggeredColor;
                    interactable = false;
                    break;
                case NodeState.Locked:
                    targetColor = lockedColor;
                    interactable = false;
                    break;
                case NodeState.Hidden:
                    SetVisible(false);
                    return;
            }
            
            nodeImage.color = targetColor;
            SetInteractable(interactable);
            
            // 更新状态指示器
            if (nodeStateIndicator != null)
            {
                nodeStateIndicator.color = targetColor;
                nodeStateIndicator.gameObject.SetActive(currentState != NodeState.Normal);
            }
        }
        
        #endregion
        
        #region 可见性和交互性控制
        
        /// <summary>
        /// 设置节点可见性
        /// </summary>
        public void SetVisible(bool visible)
        {
            isVisible = visible;
            if (canvasGroup != null)
            {
                canvasGroup.alpha = visible ? 1f : 0f;
                canvasGroup.blocksRaycasts = visible;
            }
            else
            {
                gameObject.SetActive(visible);
            }
        }
        
        /// <summary>
        /// 获取节点可见性
        /// </summary>
        public bool IsVisible()
        {
            return isVisible;
        }
        
        /// <summary>
        /// 设置节点交互性
        /// </summary>
        public void SetInteractable(bool interactable)
        {
            isInteractable = interactable;
            if (nodeButton != null)
            {
                nodeButton.interactable = interactable && isVisible;
            }
            
            if (canvasGroup != null)
            {
                canvasGroup.interactable = interactable;
            }
        }
        
        /// <summary>
        /// 获取节点交互性
        /// </summary>
        public bool IsInteractable()
        {
            return isInteractable;
        }
        
        #endregion
        
        #region 事件处理
        
        public void OnPointerClick(PointerEventData eventData)
        {
            if (isInteractable && isVisible)
            {
                OnNodeClicked?.Invoke(this);
            }
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (isInteractable && isVisible)
            {
                OnNodeHovered?.Invoke(this);
                StartHoverAnimation();
                ShowTooltip();
            }
        }
        
        public void OnPointerExit(PointerEventData eventData)
        {
            if (isInteractable && isVisible)
            {
                OnNodeUnhovered?.Invoke(this);
                StopHoverAnimation();
                HideTooltip();
            }
        }
        
        #endregion
        
        #region 动画效果
        
        /// <summary>
        /// 开始悬停动画
        /// </summary>
        private void StartHoverAnimation()
        {
            if (hoverAnimation != null)
                StopCoroutine(hoverAnimation);
            
            hoverAnimation = StartCoroutine(PlayHoverAnimation(true));
        }
        
        /// <summary>
        /// 停止悬停动画
        /// </summary>
        private void StopHoverAnimation()
        {
            if (hoverAnimation != null)
                StopCoroutine(hoverAnimation);
            
            hoverAnimation = StartCoroutine(PlayHoverAnimation(false));
        }
        
        /// <summary>
        /// 播放悬停动画
        /// </summary>
        private IEnumerator PlayHoverAnimation(bool hoverIn)
        {
            Vector3 startScale = transform.localScale;
            Vector3 targetScale = hoverIn ? originalScale * hoverScale : originalScale;
            
            float elapsedTime = 0f;
            while (elapsedTime < hoverDuration)
            {
                elapsedTime += Time.deltaTime;
                float progress = elapsedTime / hoverDuration;
                float curveValue = hoverCurve.Evaluate(progress);
                
                transform.localScale = Vector3.Lerp(startScale, targetScale, curveValue);
                yield return null;
            }
            
            transform.localScale = targetScale;
            hoverAnimation = null;
        }
        
        #endregion
        
        #region 工具提示
        
        /// <summary>
        /// 显示工具提示
        /// </summary>
        private void ShowTooltip()
        {
            if (nodeTooltip != null && nodeData?.NodeDataPersistent != null)
            {
                nodeTooltip.SetActive(true);
                // 这里可以更新工具提示的内容
                UpdateTooltipContent();
            }
        }
        
        /// <summary>
        /// 隐藏工具提示
        /// </summary>
        private void HideTooltip()
        {
            if (nodeTooltip != null)
            {
                nodeTooltip.SetActive(false);
            }
        }
        
        /// <summary>
        /// 更新工具提示内容
        /// </summary>
        private void UpdateTooltipContent()
        {
            if (nodeTooltip == null || nodeData?.NodeDataPersistent == null) return;
            
            // 查找工具提示中的文本组件并更新内容
            var tooltipText = nodeTooltip.GetComponentInChildren<TextMeshProUGUI>();
            if (tooltipText != null)
            {
                string content = $"节点: {nodeData.NodeDataPersistent.nodeName}\n";
                content += $"状态: {GetStateDisplayName(currentState)}\n";
                content += $"深度: {depth}";
                
                tooltipText.text = content;
            }
        }
        
        /// <summary>
        /// 获取状态显示名称
        /// </summary>
        private string GetStateDisplayName(NodeState state)
        {
            switch (state)
            {
                case NodeState.Normal: return "正常";
                case NodeState.Triggerable: return "可触发";
                case NodeState.Triggered: return "已触发";
                case NodeState.Locked: return "锁定";
                case NodeState.Hidden: return "隐藏";
                default: return "未知";
            }
        }
        
        #endregion
        
        #region 公共方法
        
        /// <summary>
        /// 获取当前状态
        /// </summary>
        public NodeState GetCurrentState()
        {
            return currentState;
        }
        
        /// <summary>
        /// 强制更新显示
        /// </summary>
        public void ForceUpdateDisplay()
        {
            UpdateNodeDisplay();
            UpdateNodeState();
        }
        
        #endregion
        
        private void OnDestroy()
        {
            // 清理事件
            OnNodeClicked = null;
            OnNodeHovered = null;
            OnNodeUnhovered = null;
            
            // 停止所有协程
            if (hoverAnimation != null)
                StopCoroutine(hoverAnimation);
        }
    }
    
    /// <summary>
    /// 节点状态枚举
    /// </summary>
    public enum NodeState
    {
        Normal,      // 正常状态
        Triggerable, // 可触发状态
        Triggered,   // 已触发状态
        Locked,      // 锁定状态
        Hidden       // 隐藏状态
    }
}
