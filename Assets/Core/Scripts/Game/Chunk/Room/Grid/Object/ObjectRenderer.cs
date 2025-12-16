using System.Collections.Generic;
using Core.Game.Chunk.Room.Grid.Editor;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Game.Chunk.Room.Grid.Renderer
{
    /// <summary>
    /// 占位符标记组件
    /// 用于标识物品是占位符而非真实预制体
    /// </summary>
    public class PlaceholderMarker : MonoBehaviour { }

    /// <summary>
    /// 物品渲染实例
    /// 管理单个放置物品的GameObject
    /// </summary>
    public class ObjectRenderInstance
    {
        public string InstanceId;
        public string ObjectDefId;
        public GameObject GameObject;
        public MeshRenderer MeshRenderer;
        public Collider Collider;
        public bool IsSelected;
        public bool IsHighlighted;
    }

    /// <summary>
    /// 物品渲染器
    /// 负责渲染场景中放置的所有物品
    /// </summary>
    public class ObjectRenderer : MonoBehaviour
    {
        #region 配置

        [Title("引用")]
        
        [LabelText("编辑器")]
        [SerializeField]
        private RoomGridEditor _editor;

        [Title("渲染设置")]
        
        [LabelText("物品根节点")]
        [SerializeField]
        private Transform _objectRoot;

        [LabelText("默认物品材质")]
        [SerializeField]
        private Material _defaultMaterial;

        [LabelText("选中高亮材质")]
        [SerializeField]
        private Material _selectedMaterial;

        [LabelText("悬停高亮材质")]
        [SerializeField]
        private Material _highlightMaterial;

        [Title("占位符设置")]
        
        [LabelText("使用占位符")]
        [PropertyTooltip("当没有预制体时使用占位符")]
        [SerializeField]
        private bool _usePlaceholder = true;

        [LabelText("占位符颜色")]
        [SerializeField]
        private Color _placeholderColor = new Color(0.6f, 0.6f, 0.8f, 1f);

        #endregion

        #region 运行时数据

        [Title("调试信息")]
        
        [LabelText("渲染物品数")]
        [ReadOnly]
        [ShowInInspector]
        private int _renderedObjectCount;

        /// <summary>
        /// 物品渲染实例字典
        /// </summary>
        private Dictionary<string, ObjectRenderInstance> _instances = new Dictionary<string, ObjectRenderInstance>();

        /// <summary>
        /// 当前选中的物品ID
        /// </summary>
        private string _selectedInstanceId;

        /// <summary>
        /// 当前高亮的物品ID
        /// </summary>
        private string _highlightedInstanceId;

        /// <summary>
        /// 物品定义管理器
        /// </summary>
        private ObjectDefinitionManager _defManager;

        #endregion

        #region 生命周期

        private void Awake()
        {
            if (_objectRoot == null)
            {
                _objectRoot = new GameObject("ObjectRoot").transform;
                _objectRoot.SetParent(transform);
                _objectRoot.localPosition = Vector3.zero;
            }

            CreateDefaultMaterials();
        }

        private void Start()
        {
            _defManager = ObjectDefinitionManager.Instance;

            if (_editor == null)
            {
                _editor = GetComponent<RoomGridEditor>();
            }

            if (_editor == null)
            {
                _editor = GetComponentInParent<RoomGridEditor>();
            }

            if (_editor != null)
            {
                SubscribeEvents();

                if (_editor.IsInitialized)
                {
                    RebuildAll();
                }
            }
        }

        /// <summary>
        /// 设置编辑器引用
        /// </summary>
        public void SetEditor(RoomGridEditor editor)
        {
            if (_editor == editor) return;

            // 先取消之前的订阅
            UnsubscribeEvents();

            _editor = editor;
            _defManager = ObjectDefinitionManager.Instance;

            if (_editor != null)
            {
                SubscribeEvents();

                // 如果编辑器已初始化，重建所有物品
                if (_editor.IsInitialized)
                {
                    RebuildAll();
                }
            }

            Debug.Log($"[ObjectRenderer] 编辑器引用已设置: {(_editor != null ? "有效" : "null")}");
        }

        private void OnDestroy()
        {
            UnsubscribeEvents();
            ClearAll();
        }

        #endregion

        #region 初始化

        /// <summary>
        /// 创建默认材质
        /// </summary>
        private void CreateDefaultMaterials()
        {
            var shader = Shader.Find("Standard");

            if (_defaultMaterial == null)
            {
                _defaultMaterial = new Material(shader);
                _defaultMaterial.color = _placeholderColor;
                _defaultMaterial.name = "Object_Default";
            }

            if (_selectedMaterial == null)
            {
                _selectedMaterial = new Material(shader);
                _selectedMaterial.color = new Color(1f, 0.8f, 0.2f);
                _selectedMaterial.name = "Object_Selected";
                _selectedMaterial.EnableKeyword("_EMISSION");
                _selectedMaterial.SetColor("_EmissionColor", new Color(0.3f, 0.24f, 0.06f));
            }

            if (_highlightMaterial == null)
            {
                _highlightMaterial = new Material(shader);
                _highlightMaterial.color = new Color(0.8f, 1f, 0.8f);
                _highlightMaterial.name = "Object_Highlight";
                _highlightMaterial.EnableKeyword("_EMISSION");
                _highlightMaterial.SetColor("_EmissionColor", new Color(0.1f, 0.2f, 0.1f));
            }
        }

        /// <summary>
        /// 订阅事件
        /// </summary>
        private void SubscribeEvents()
        {
            if (_editor == null) return;

            _editor.OnEditorInitialized += OnEditorInitialized;
            _editor.OnObjectPlaced += OnObjectPlaced;
            _editor.OnObjectRemoved += OnObjectRemoved;
        }

        /// <summary>
        /// 取消订阅
        /// </summary>
        private void UnsubscribeEvents()
        {
            if (_editor == null) return;

            _editor.OnEditorInitialized -= OnEditorInitialized;
            _editor.OnObjectPlaced -= OnObjectPlaced;
            _editor.OnObjectRemoved -= OnObjectRemoved;
        }

        #endregion

        #region 事件处理

        private void OnEditorInitialized()
        {
            Debug.Log("[ObjectRenderer] 编辑器初始化完成，重建物品渲染");
            RebuildAll();
        }

        private void OnObjectPlaced(PlacedObjectData obj)
        {
            Debug.Log($"[ObjectRenderer] 物品放置: {obj.InstanceId}");
            CreateObjectInstance(obj);
        }

        private void OnObjectRemoved(PlacedObjectData obj)
        {
            Debug.Log($"[ObjectRenderer] 物品移除: {obj.InstanceId}");
            RemoveObjectInstance(obj.InstanceId);
        }

        #endregion

        #region 渲染管理

        /// <summary>
        /// 重建所有物品
        /// </summary>
        [Button("重建所有物品", ButtonSizes.Large)]
        public void RebuildAll()
        {
            ClearAll();

            if (_editor?.Grid == null) return;

            var objects = _editor.Grid.GetAllObjects();
            foreach (var obj in objects)
            {
                CreateObjectInstance(obj);
            }

            _renderedObjectCount = _instances.Count;
            Debug.Log($"[ObjectRenderer] 重建完成: {_renderedObjectCount} 个物品");
        }

        /// <summary>
        /// 创建物品实例
        /// </summary>
        private void CreateObjectInstance(PlacedObjectData objData)
        {
            if (objData == null || string.IsNullOrEmpty(objData.InstanceId)) return;

            // 已存在则先移除
            if (_instances.ContainsKey(objData.InstanceId))
            {
                RemoveObjectInstance(objData.InstanceId);
            }

            // 获取物品定义
            var def = _defManager?.GetDefinition(objData.ObjectDefId);
            
            // 创建GameObject
            GameObject go = null;
            
            // 尝试加载预制体
            if (def != null && !string.IsNullOrEmpty(def.PrefabPath))
            {
                // TODO: 通过YooAsset加载预制体
                // go = YooAsset.LoadAsset<GameObject>(def.PrefabPath);
            }

            // 使用占位符
            if (go == null && _usePlaceholder)
            {
                go = CreatePlaceholderObject(objData, def);
            }

            if (go == null) return;

            // 设置Transform
            var config = _editor.Grid.Config;
            Vector3 worldPos = objData.GetWorldPosition(config.TileSize);
            worldPos.y += (def?.YOffset ?? 0f);
            
            go.transform.SetParent(_objectRoot);
            go.transform.position = worldPos;
            
            // 占位符已经使用ActualSize创建，不需要旋转
            // 真实预制体需要旋转
            bool isPlaceholder = go.GetComponent<PlaceholderMarker>() != null;
            if (!isPlaceholder)
            {
                go.transform.rotation = objData.RotationQuaternion;
            }
            
            go.name = $"Object_{objData.InstanceId}";

            // 获取组件
            var meshRenderer = go.GetComponentInChildren<MeshRenderer>();
            var collider = go.GetComponentInChildren<Collider>();

            // 添加碰撞体（如果没有）
            if (collider == null)
            {
                collider = go.AddComponent<BoxCollider>();
            }

            // 创建渲染实例
            var instance = new ObjectRenderInstance
            {
                InstanceId = objData.InstanceId,
                ObjectDefId = objData.ObjectDefId,
                GameObject = go,
                MeshRenderer = meshRenderer,
                Collider = collider
            };

            _instances[objData.InstanceId] = instance;
            _renderedObjectCount = _instances.Count;
        }

        /// <summary>
        /// 创建占位符物品
        /// </summary>
        private GameObject CreatePlaceholderObject(PlacedObjectData objData, ObjectDefinition def)
        {
            var go = new GameObject();
            
            // 使用旋转后的实际尺寸（占位符不需要再旋转）
            var size = objData.ActualSize;
            var config = _editor.Grid.Config;
            
            float width = size.Width * config.TileSize * 0.9f;
            float depth = size.Depth * config.TileSize * 0.9f;
            float height = size.Height;

            // 创建主体
            var bodyGO = GameObject.CreatePrimitive(PrimitiveType.Cube);
            bodyGO.transform.SetParent(go.transform);
            bodyGO.transform.localPosition = new Vector3(0, height * 0.5f, 0);
            bodyGO.transform.localScale = new Vector3(width, height, depth);
            
            // 设置材质
            var renderer = bodyGO.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                var mat = new Material(_defaultMaterial);
                
                // 根据类别设置颜色
                Color color = _placeholderColor;
                if (def != null)
                {
                    switch (def.Category)
                    {
                        case ObjectCategory.Furniture:
                            color = new Color(0.6f, 0.4f, 0.2f);
                            break;
                        case ObjectCategory.Decoration:
                            color = new Color(0.8f, 0.6f, 0.8f);
                            break;
                        case ObjectCategory.Plant:
                            color = new Color(0.3f, 0.7f, 0.3f);
                            break;
                        case ObjectCategory.Lighting:
                            color = new Color(1f, 0.9f, 0.5f);
                            break;
                        case ObjectCategory.Storage:
                            color = new Color(0.5f, 0.5f, 0.6f);
                            break;
                        case ObjectCategory.Interactive:
                            color = new Color(0.4f, 0.6f, 0.9f);
                            break;
                    }
                }
                
                mat.color = color;
                renderer.material = mat;
            }

            // 移除Cube自带的碰撞体（稍后添加完整的）
            var cubeCollider = bodyGO.GetComponent<Collider>();
            if (cubeCollider != null)
            {
                Destroy(cubeCollider);
            }

            // 添加整体碰撞体
            var boxCollider = go.AddComponent<BoxCollider>();
            boxCollider.center = new Vector3(0, height * 0.5f, 0);
            boxCollider.size = new Vector3(width, height, depth);

            // 标记为占位符（不需要Transform旋转）
            go.AddComponent<PlaceholderMarker>();

            return go;
        }

        /// <summary>
        /// 移除物品实例
        /// </summary>
        private void RemoveObjectInstance(string instanceId)
        {
            if (string.IsNullOrEmpty(instanceId)) return;

            if (_instances.TryGetValue(instanceId, out var instance))
            {
                if (instance.GameObject != null)
                {
                    Destroy(instance.GameObject);
                }
                _instances.Remove(instanceId);
            }

            _renderedObjectCount = _instances.Count;
        }

        /// <summary>
        /// 清除所有物品
        /// </summary>
        [Button("清除所有")]
        public void ClearAll()
        {
            foreach (var instance in _instances.Values)
            {
                if (instance.GameObject != null)
                {
                    Destroy(instance.GameObject);
                }
            }
            _instances.Clear();
            _renderedObjectCount = 0;
            _selectedInstanceId = null;
            _highlightedInstanceId = null;
        }

        #endregion

        #region 选择与高亮

        /// <summary>
        /// 选中物品
        /// </summary>
        public void SelectObject(string instanceId)
        {
            // 取消之前的选中
            if (!string.IsNullOrEmpty(_selectedInstanceId) && _instances.TryGetValue(_selectedInstanceId, out var prevSelected))
            {
                prevSelected.IsSelected = false;
                UpdateObjectMaterial(prevSelected);
            }

            _selectedInstanceId = instanceId;

            // 设置新的选中
            if (!string.IsNullOrEmpty(instanceId) && _instances.TryGetValue(instanceId, out var newSelected))
            {
                newSelected.IsSelected = true;
                UpdateObjectMaterial(newSelected);
            }
        }

        /// <summary>
        /// 取消选中
        /// </summary>
        public void DeselectObject()
        {
            SelectObject(null);
        }

        /// <summary>
        /// 高亮物品（悬停）
        /// </summary>
        public void HighlightObject(string instanceId)
        {
            // 取消之前的高亮
            if (!string.IsNullOrEmpty(_highlightedInstanceId) && _instances.TryGetValue(_highlightedInstanceId, out var prevHighlighted))
            {
                prevHighlighted.IsHighlighted = false;
                UpdateObjectMaterial(prevHighlighted);
            }

            _highlightedInstanceId = instanceId;

            // 设置新的高亮
            if (!string.IsNullOrEmpty(instanceId) && _instances.TryGetValue(instanceId, out var newHighlighted))
            {
                newHighlighted.IsHighlighted = true;
                UpdateObjectMaterial(newHighlighted);
            }
        }

        /// <summary>
        /// 取消高亮
        /// </summary>
        public void ClearHighlight()
        {
            HighlightObject(null);
        }

        /// <summary>
        /// 更新物品材质
        /// </summary>
        private void UpdateObjectMaterial(ObjectRenderInstance instance)
        {
            if (instance?.MeshRenderer == null) return;

            Material mat;
            if (instance.IsSelected)
            {
                mat = _selectedMaterial;
            }
            else if (instance.IsHighlighted)
            {
                mat = _highlightMaterial;
            }
            else
            {
                // 恢复原始材质
                mat = _defaultMaterial;
            }

            instance.MeshRenderer.material = mat;
        }

        #endregion

        #region 查询

        /// <summary>
        /// 获取渲染实例
        /// </summary>
        public ObjectRenderInstance GetInstance(string instanceId)
        {
            _instances.TryGetValue(instanceId, out var instance);
            return instance;
        }

        /// <summary>
        /// 通过位置查找物品
        /// </summary>
        public ObjectRenderInstance GetInstanceAtPosition(Vector3 worldPos)
        {
            if (_editor?.Grid == null) return null;

            var tilePos = _editor.Grid.Config.WorldToTile(worldPos);
            var objData = _editor.Grid.GetObjectAtPosition(tilePos);
            
            if (objData != null)
            {
                return GetInstance(objData.InstanceId);
            }

            return null;
        }

        /// <summary>
        /// 射线检测物品
        /// </summary>
        public ObjectRenderInstance RaycastObject(Ray ray, out RaycastHit hit)
        {
            hit = default;

            if (Physics.Raycast(ray, out hit, 1000f))
            {
                var go = hit.collider.gameObject;
                
                // 查找是否是物品实例
                foreach (var instance in _instances.Values)
                {
                    if (instance.GameObject == go || 
                        (go.transform.IsChildOf(instance.GameObject.transform)))
                    {
                        return instance;
                    }
                }
            }

            return null;
        }

        #endregion

        #region 更新物品

        /// <summary>
        /// 更新物品位置
        /// </summary>
        public void UpdateObjectPosition(string instanceId)
        {
            if (_editor?.Grid == null) return;

            var objData = _editor.Grid.GetObjectByInstanceId(instanceId);
            if (objData == null) return;

            if (_instances.TryGetValue(instanceId, out var instance))
            {
                var config = _editor.Grid.Config;
                var def = _defManager?.GetDefinition(objData.ObjectDefId);
                
                Vector3 worldPos = objData.GetWorldPosition(config.TileSize);
                worldPos.y += (def?.YOffset ?? 0f);
                
                instance.GameObject.transform.position = worldPos;
                instance.GameObject.transform.rotation = objData.RotationQuaternion;
            }
        }

        /// <summary>
        /// 刷新指定楼层的物品
        /// </summary>
        public void RefreshFloor(int floor)
        {
            if (_editor?.Grid == null) return;

            // 获取当前楼层的所有物品
            var allObjects = _editor.Grid.GetAllObjects();
            
            // 移除不属于当前楼层的物品渲染
            var toRemove = new List<string>();
            foreach (var kvp in _instances)
            {
                var objData = _editor.Grid.GetObjectByInstanceId(kvp.Key);
                if (objData == null || objData.FloorLevel != floor)
                {
                    toRemove.Add(kvp.Key);
                }
            }

            foreach (var id in toRemove)
            {
                if (_instances.TryGetValue(id, out var instance))
                {
                    instance.GameObject?.SetActive(false);
                }
            }

            // 确保当前楼层的物品都已渲染
            foreach (var obj in allObjects)
            {
                if (obj.FloorLevel == floor)
                {
                    if (_instances.TryGetValue(obj.InstanceId, out var instance))
                    {
                        instance.GameObject?.SetActive(true);
                    }
                    else
                    {
                        CreateObjectInstance(obj);
                    }
                }
            }
        }

        #endregion
    }
}