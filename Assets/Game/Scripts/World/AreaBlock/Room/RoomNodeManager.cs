using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Game.World
{
    public class RoomNodeManager : MonoBehaviour
    {
        [Header("节点管理")]
        [SerializeField] private Transform nodeContainer;
        [SerializeField] private GameObject nodeComponentPrefab;
        [SerializeField] private BatchConnectionLines connectionManager;
        
        [Header("节点显示设置")]
        [SerializeField] private float nodeSpacing = 100f;
        [SerializeField] private float levelSpacing = 150f;
        [SerializeField] private bool autoArrangeNodes = true;
        
        [Header("动画设置")]
        [SerializeField] private float nodeAppearDuration = 0.5f;
        [SerializeField] private AnimationCurve nodeAppearCurve;
        
        // 存储所有节点组件
        private Dictionary<string, NodeComponent> nodeComponents = new Dictionary<string, NodeComponent>();
        // 存储节点连接关系映射
        private Dictionary<string, List<int>> nodeConnectionMap = new Dictionary<string, List<int>>();
        // 根节点列表
        private List<NodeData> rootNodes = new List<NodeData>();
        
        public event Action<NodeData> OnNodeTriggered;
        public event Action<NodeData> OnNodeShown;
        
        private void Start()
        {
            InitializeNodeManager();
        }
        
        private void InitializeNodeManager()
        {
            if (connectionManager == null)
            {
                connectionManager = GetComponent<BatchConnectionLines>();
                if (connectionManager == null)
                {
                    Debug.LogError("NodeManager: BatchConnectionLines component not found!");
                    return;
                }
            }
            
            if (nodeContainer == null)
            {
                nodeContainer = transform;
            }
        }
        
        #region 节点创建和管理
        
        /// <summary>
        /// 添加根节点
        /// </summary>
        public NodeComponent AddRootNode(NodeDataPersistent nodeDataPersistent, Vector2? position = null)
        {
            var nodeData = new NodeData(nodeDataPersistent);
            rootNodes.Add(nodeData);
            
            Vector2 spawnPosition = position ?? CalculateRootNodePosition(rootNodes.Count - 1);
            return CreateNodeComponent(nodeData, spawnPosition, 0);
        }
        
        /// <summary>
        /// 创建节点组件
        /// </summary>
        private NodeComponent CreateNodeComponent(NodeData nodeData, Vector2 position, int depth)
        {
            if (nodeComponents.ContainsKey(nodeData.NodeDataPersistent.nodeId))
            {
                Debug.LogWarning($"Node with ID {nodeData.NodeDataPersistent.nodeId} already exists!");
                return nodeComponents[nodeData.NodeDataPersistent.nodeId];
            }
            
            GameObject nodeObj = Instantiate(nodeComponentPrefab, nodeContainer);
            NodeComponent nodeComponent = nodeObj.GetComponent<NodeComponent>();
            
            if (nodeComponent == null)
            {
                nodeComponent = nodeObj.AddComponent<NodeComponent>();
            }
            
            // 设置节点数据
            nodeComponent.SetNodeData(nodeData);
            nodeComponent.SetDepth(depth);
            
            // 设置位置
            RectTransform rectTransform = nodeObj.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = nodeData.NodeDataPersistent.nodePosition = position;
            
            // 注册点击事件
            nodeComponent.OnNodeClicked += HandleNodeClicked;
            
            // 存储节点组件
            nodeComponents[nodeData.NodeDataPersistent.nodeId] = nodeComponent;
            nodeConnectionMap[nodeData.NodeDataPersistent.nodeId] = new List<int>();
            
            // 播放出现动画
            if (depth > 0) // 根节点不播放动画
            {
                StartCoroutine(PlayNodeAppearAnimation(nodeComponent));
            }
            else
            {
                nodeComponent.SetVisible(true);
            }
            
            return nodeComponent;
        }
        
        /// <summary>
        /// 处理节点点击事件
        /// </summary>
        private void HandleNodeClicked(NodeComponent nodeComponent)
        {
            NodeData nodeData = nodeComponent.GetNodeData();
            
            // 检查触发条件
            if (CanTriggerNode(nodeData))
            {
                TriggerNode(nodeData);
            }
            else
            {
                Debug.Log($"Node {nodeData.NodeDataPersistent.nodeName} cannot be triggered yet.");
                // 可以在这里添加提示效果
                StartCoroutine(PlayNodeFailAnimation(nodeComponent));
            }
        }
        
        /// <summary>
        /// 检查节点是否可以触发
        /// </summary>
        private bool CanTriggerNode(NodeData nodeData)
        {
            // 如果已经触发过，不能再次触发
            if (nodeData.NodeDataTemporary.hasBeanTrigger)
            {
                return false;
            }
            
            // 检查父节点是否都已触发
            if (nodeData.NodeDataPersistent.parentNodeData != null)
            {
                string parentId = nodeData.NodeDataPersistent.parentNodeData.nodeId;
                if (nodeComponents.ContainsKey(parentId))
                {
                    var parentNodeData = nodeComponents[parentId].GetNodeData();
                    if (!parentNodeData.NodeDataTemporary.hasBeanTrigger)
                    {
                        return false;
                    }
                }
            }
            
            // 检查触发条件（如果有的话）
            if (nodeData.NodeDataPersistent.actionTriggerData?.actionTriggerCondition != null)
            {
                return CheckActionTriggerCondition(nodeData.NodeDataPersistent.actionTriggerData.actionTriggerCondition);
            }
            
            return true;
        }
        
        /// <summary>
        /// 检查行为触发条件
        /// </summary>
        private bool CheckActionTriggerCondition(ActionTriggerCondition condition)
        {
            // TODO: 实现具体的条件检查逻辑
            // 这里可以根据你的具体需求来实现条件检查
            return true;
        }
        
        /// <summary>
        /// 触发节点
        /// </summary>
        private void TriggerNode(NodeData nodeData)
        {
            // 标记为已触发
            nodeData.NodeDataTemporary.hasBeanTrigger = true;
            
            // 执行触发效果
            ExecuteNodeTriggerEffects(nodeData);
            
            // 生成子节点
            GenerateChildNodes(nodeData);
            
            // 触发事件
            OnNodeTriggered?.Invoke(nodeData);
            
            // 更新节点显示状态
            if (nodeComponents.ContainsKey(nodeData.NodeDataPersistent.nodeId))
            {
                nodeComponents[nodeData.NodeDataPersistent.nodeId].UpdateNodeState();
            }
            
            Debug.Log($"Node {nodeData.NodeDataPersistent.nodeName} triggered successfully!");
        }
        
        /// <summary>
        /// 执行节点触发效果
        /// </summary>
        private void ExecuteNodeTriggerEffects(NodeData nodeData)
        {
            var actionData = nodeData.NodeDataPersistent.actionTriggerData;
            if (actionData == null) return;
            
            // 震动屏幕
            if (actionData.willShakeScreen)
            {
                StartCoroutine(ShakeScreen(actionData.shakeSceenStrength));
            }
            
            // 播放音频
            if (actionData.audioClip != null)
            {
                PlayAudio(actionData.audioClip);
            }
            
            // 生成粒子特效
            if (actionData.particleObject != null)
            {
                SpawnParticleEffect(actionData.particleObject, 
                    nodeData.NodeDataPersistent.nodePosition + actionData.particlePos);
            }
        }
        
        /// <summary>
        /// 生成子节点
        /// </summary>
        private void GenerateChildNodes(NodeData parentNodeData)
        {
            if (parentNodeData.NodeDataPersistent.childNodeDataList == null || 
                parentNodeData.NodeDataPersistent.childNodeDataList.Count == 0)
            {
                return;
            }
            
            int childCount = parentNodeData.NodeDataPersistent.childNodeDataList.Count;
            int parentDepth = GetNodeDepth(parentNodeData.NodeDataPersistent.nodeId);
            
            for (int i = 0; i < childCount; i++)
            {
                var childNodeDataPersistent = parentNodeData.NodeDataPersistent.childNodeDataList[i];
                
                // 如果子节点已经存在，只显示它
                if (nodeComponents.ContainsKey(childNodeDataPersistent.nodeId))
                {
                    var existingChild = nodeComponents[childNodeDataPersistent.nodeId];
                    if (!existingChild.IsVisible())
                    {
                        ShowNode(existingChild);
                    }
                    continue;
                }
                
                // 计算子节点位置
                Vector2 childPosition = CalculateChildNodePosition(
                    parentNodeData.NodeDataPersistent.nodePosition, 
                    i, 
                    childCount, 
                    parentDepth + 1);
                
                // 创建子节点
                var childNodeData = new NodeData(childNodeDataPersistent);
                NodeComponent childComponent = CreateNodeComponent(childNodeData, childPosition, parentDepth + 1);
                
                // 创建连接线
                CreateConnection(parentNodeData.NodeDataPersistent.nodeId, childNodeDataPersistent.nodeId);
                
                // 延迟显示效果
                StartCoroutine(DelayedShowNode(childComponent, i * 0.1f));
            }
        }
        
        #endregion
        
        #region 节点布局和位置计算
        
        /// <summary>
        /// 计算根节点位置
        /// </summary>
        private Vector2 CalculateRootNodePosition(int index)
        {
            if (!autoArrangeNodes) return Vector2.zero;
            
            return new Vector2(index * nodeSpacing, 0);
        }
        
        /// <summary>
        /// 计算子节点位置
        /// </summary>
        private Vector2 CalculateChildNodePosition(Vector2 parentPosition, int childIndex, int totalChildren, int depth)
        {
            if (!autoArrangeNodes) return parentPosition + Vector2.down * levelSpacing;
            
            float totalWidth = (totalChildren - 1) * nodeSpacing;
            float startX = parentPosition.x - totalWidth * 0.5f;
            float childX = startX + childIndex * nodeSpacing;
            float childY = parentPosition.y - levelSpacing;
            
            return new Vector2(childX, childY);
        }
        
        /// <summary>
        /// 重新排列所有节点
        /// </summary>
        public void RearrangeAllNodes()
        {
            if (!autoArrangeNodes) return;
            
            // 重新排列根节点
            for (int i = 0; i < rootNodes.Count; i++)
            {
                Vector2 newPosition = CalculateRootNodePosition(i);
                UpdateNodePosition(rootNodes[i].NodeDataPersistent.nodeId, newPosition);
                RearrangeNodeHierarchy(rootNodes[i], 0);
            }
            
            // 更新所有连接线
            connectionManager.SetDirtyAll();
        }
        
        /// <summary>
        /// 递归重排节点层次结构
        /// </summary>
        private void RearrangeNodeHierarchy(NodeData nodeData, int depth)
        {
            if (nodeData.NodeDataPersistent.childNodeDataList == null) return;
            
            int childCount = nodeData.NodeDataPersistent.childNodeDataList.Count;
            for (int i = 0; i < childCount; i++)
            {
                var childNodeDataPersistent = nodeData.NodeDataPersistent.childNodeDataList[i];
                if (nodeComponents.ContainsKey(childNodeDataPersistent.nodeId))
                {
                    Vector2 newPosition = CalculateChildNodePosition(
                        nodeData.NodeDataPersistent.nodePosition, 
                        i, 
                        childCount, 
                        depth + 1);
                    
                    UpdateNodePosition(childNodeDataPersistent.nodeId, newPosition);
                    
                    var childNodeData = nodeComponents[childNodeDataPersistent.nodeId].GetNodeData();
                    RearrangeNodeHierarchy(childNodeData, depth + 1);
                }
            }
        }
        
        #endregion
        
        #region 连接线管理
        
        /// <summary>
        /// 创建节点间的连接
        /// </summary>
        private void CreateConnection(string parentId, string childId)
        {
            if (!nodeComponents.ContainsKey(parentId) || !nodeComponents.ContainsKey(childId))
            {
                Debug.LogWarning($"Cannot create connection: Node not found (Parent: {parentId}, Child: {childId})");
                return;
            }
            
            var parentTransform = nodeComponents[parentId].GetComponent<RectTransform>();
            var childTransform = nodeComponents[childId].GetComponent<RectTransform>();
            
            // 创建连接线
            int connectionIndex = connectionManager.AddArrowConnection(
                parentTransform, 
                childTransform,
                ArrowType.Triangle,
                ArrowPosition.End,
                10f, 
                3f,
                Color.cyan);
            
            // 存储连接映射
            nodeConnectionMap[parentId].Add(connectionIndex);
        }
        
        /// <summary>
        /// 移除节点的所有连接
        /// </summary>
        private void RemoveNodeConnections(string nodeId)
        {
            if (nodeConnectionMap.ContainsKey(nodeId))
            {
                foreach (int connectionIndex in nodeConnectionMap[nodeId])
                {
                    connectionManager.RemoveConnection(connectionIndex);
                }
                nodeConnectionMap[nodeId].Clear();
            }
        }
        
        #endregion
        
        #region 动画和效果
        
        /// <summary>
        /// 播放节点出现动画
        /// </summary>
        private IEnumerator PlayNodeAppearAnimation(NodeComponent nodeComponent)
        {
            nodeComponent.SetVisible(false);
            Transform nodeTransform = nodeComponent.transform;
            Vector3 originalScale = nodeTransform.localScale;
            
            nodeTransform.localScale = Vector3.zero;
            nodeComponent.SetVisible(true);
            
            float elapsedTime = 0f;
            while (elapsedTime < nodeAppearDuration)
            {
                elapsedTime += Time.deltaTime;
                float progress = elapsedTime / nodeAppearDuration;
                float scaleFactor = nodeAppearCurve.Evaluate(progress);
                
                nodeTransform.localScale = originalScale * scaleFactor;
                yield return null;
            }
            
            nodeTransform.localScale = originalScale;
            
            // 标记为已显示
            var nodeData = nodeComponent.GetNodeData();
            nodeData.NodeDataTemporary.hasBeanShowing = true;
            OnNodeShown?.Invoke(nodeData);
        }
        
        /// <summary>
        /// 播放节点失败动画
        /// </summary>
        private IEnumerator PlayNodeFailAnimation(NodeComponent nodeComponent)
        {
            Transform nodeTransform = nodeComponent.transform;
            Vector3 originalPosition = nodeTransform.localPosition;
            
            float duration = 0.3f;
            float elapsedTime = 0f;
            float shakeAmount = 10f;
            
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float progress = elapsedTime / duration;
                
                Vector3 randomOffset = new Vector3(
                    UnityEngine.Random.Range(-shakeAmount, shakeAmount) * (1 - progress),
                    UnityEngine.Random.Range(-shakeAmount, shakeAmount) * (1 - progress),
                    0);
                
                nodeTransform.localPosition = originalPosition + randomOffset;
                yield return null;
            }
            
            nodeTransform.localPosition = originalPosition;
        }
        
        /// <summary>
        /// 屏幕震动效果
        /// </summary>
        private IEnumerator ShakeScreen(int strength)
        {
            // TODO: 实现屏幕震动效果
            Debug.Log($"Screen shake with strength: {strength}");
            yield return new WaitForSeconds(0.5f);
        }
        
        /// <summary>
        /// 播放音频
        /// </summary>
        private void PlayAudio(AudioClip audioClip)
        {
            // TODO: 实现音频播放
            Debug.Log($"Playing audio: {audioClip.name}");
        }
        
        /// <summary>
        /// 生成粒子特效
        /// </summary>
        private void SpawnParticleEffect(GameObject particlePrefab, Vector2 position)
        {
            // TODO: 实现粒子特效生成
            Debug.Log($"Spawning particle effect at: {position}");
        }
        
        /// <summary>
        /// 延迟显示节点
        /// </summary>
        private IEnumerator DelayedShowNode(NodeComponent nodeComponent, float delay)
        {
            yield return new WaitForSeconds(delay);
            // 节点创建时就会播放出现动画，这里不需要额外处理
        }
        
        #endregion
        
        #region 公共API
        
        /// <summary>
        /// 显示节点
        /// </summary>
        public void ShowNode(NodeComponent nodeComponent)
        {
            if (!nodeComponent.IsVisible())
            {
                StartCoroutine(PlayNodeAppearAnimation(nodeComponent));
            }
        }
        
        /// <summary>
        /// 获取节点组件
        /// </summary>
        public NodeComponent GetNodeComponent(string nodeId)
        {
            return nodeComponents.ContainsKey(nodeId) ? nodeComponents[nodeId] : null;
        }
        
        /// <summary>
        /// 更新节点位置
        /// </summary>
        public void UpdateNodePosition(string nodeId, Vector2 newPosition)
        {
            if (nodeComponents.ContainsKey(nodeId))
            {
                var rectTransform = nodeComponents[nodeId].GetComponent<RectTransform>();
                rectTransform.anchoredPosition = newPosition;
                
                // 更新数据中的位置
                var nodeData = nodeComponents[nodeId].GetNodeData();
                nodeData.NodeDataPersistent.nodePosition = newPosition;
            }
        }
        
        /// <summary>
        /// 获取节点深度
        /// </summary>
        public int GetNodeDepth(string nodeId)
        {
            if (nodeComponents.ContainsKey(nodeId))
            {
                return nodeComponents[nodeId].GetDepth();
            }
            return 0;
        }
        
        /// <summary>
        /// 清除所有节点
        /// </summary>
        public void ClearAllNodes()
        {
            foreach (var nodeComponent in nodeComponents.Values)
            {
                if (nodeComponent != null)
                {
                    DestroyImmediate(nodeComponent.gameObject);
                }
            }
            
            nodeComponents.Clear();
            nodeConnectionMap.Clear();
            rootNodes.Clear();
            connectionManager.ClearAllConnections();
        }
        
        /// <summary>
        /// 设置自动排列
        /// </summary>
        public void SetAutoArrange(bool enable)
        {
            autoArrangeNodes = enable;
            if (enable)
            {
                RearrangeAllNodes();
            }
        }
        
        /// <summary>
        /// 获取所有已触发的节点
        /// </summary>
        public List<NodeData> GetTriggeredNodes()
        {
            List<NodeData> triggeredNodes = new List<NodeData>();
            foreach (var nodeComponent in nodeComponents.Values)
            {
                var nodeData = nodeComponent.GetNodeData();
                if (nodeData.NodeDataTemporary.hasBeanTrigger)
                {
                    triggeredNodes.Add(nodeData);
                }
            }
            return triggeredNodes;
        }
        
        /// <summary>
        /// 获取所有可触发的节点
        /// </summary>
        public List<NodeData> GetTriggerableNodes()
        {
            List<NodeData> triggerableNodes = new List<NodeData>();
            foreach (var nodeComponent in nodeComponents.Values)
            {
                var nodeData = nodeComponent.GetNodeData();
                if (CanTriggerNode(nodeData))
                {
                    triggerableNodes.Add(nodeData);
                }
            }
            return triggerableNodes;
        }
        
        #endregion
    }
}