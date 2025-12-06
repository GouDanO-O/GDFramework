using System.Collections.Generic;
using Core.Game.Chunk.Room.Grid.Editor;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Game.Chunk.Room.Grid.Renderer
{
    /// <summary>
    /// 预览渲染器
    /// 显示地块/物品放置前的预览效果
    /// </summary>
    public class PreviewRenderer : MonoBehaviour
    {
        #region 配置

        [Title("引用")]
        
        [LabelText("编辑器")]
        [SerializeField]
        private RoomGridEditor _editor;

        [Title("预览设置")]
        
        [LabelText("有效预览颜色")]
        [SerializeField]
        private Color _validColor = new Color(0f, 1f, 0f, 0.5f);

        [LabelText("无效预览颜色")]
        [SerializeField]
        private Color _invalidColor = new Color(1f, 0f, 0f, 0.5f);

        [LabelText("预览高度偏移")]
        [SerializeField]
        private float _previewHeightOffset = 0.05f;

        [LabelText("预览边框宽度")]
        [SerializeField]
        private float _borderWidth = 0.05f;

        [Title("材质")]
        
        [LabelText("预览材质")]
        [SerializeField]
        private Material _previewMaterial;

        #endregion

        #region 运行时数据

        private GameObject _previewObject;
        private MeshFilter _meshFilter;
        private MeshRenderer _meshRenderer;
        private Mesh _previewMesh;
        
        private List<TilePosition> _currentPreviewPositions = new List<TilePosition>();
        private bool _isPreviewValid;

        #endregion

        #region 生命周期

        private void Awake()
        {
            CreatePreviewObject();
        }

        private void Start()
        {
            if (_editor == null)
            {
                _editor = GetComponent<RoomGridEditor>();
            }

            if (_editor == null)
            {
                _editor = GetComponentInParent<RoomGridEditor>();
            }
        }

        private void Update()
        {
            UpdatePreview();
        }

        private void OnDestroy()
        {
            if (_previewMesh != null)
            {
                Destroy(_previewMesh);
            }
            if (_previewObject != null)
            {
                Destroy(_previewObject);
            }
        }

        #endregion

        #region 初始化

        /// <summary>
        /// 创建预览对象
        /// </summary>
        private void CreatePreviewObject()
        {
            _previewObject = new GameObject("TilePreview");
            _previewObject.transform.SetParent(transform);
            _previewObject.transform.localPosition = Vector3.zero;

            _meshFilter = _previewObject.AddComponent<MeshFilter>();
            _meshRenderer = _previewObject.AddComponent<MeshRenderer>();

            // 创建预览材质
            if (_previewMaterial == null)
            {
                _previewMaterial = CreateDefaultPreviewMaterial();
            }

            _meshRenderer.material = _previewMaterial;
            _meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            _meshRenderer.receiveShadows = false;

            // 创建网格
            _previewMesh = new Mesh();
            _previewMesh.name = "PreviewMesh";
            _meshFilter.mesh = _previewMesh;

            // 初始隐藏
            _previewObject.SetActive(false);
        }

        /// <summary>
        /// 创建默认预览材质
        /// </summary>
        private Material CreateDefaultPreviewMaterial()
        {
            // 使用透明着色器
            var shader = Shader.Find("Sprites/Default");
            if (shader == null)
            {
                shader = Shader.Find("Standard");
            }

            var mat = new Material(shader);
            mat.color = _validColor;
            
            // 设置透明渲染
            mat.SetFloat("_Mode", 3); // Transparent
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);
            mat.DisableKeyword("_ALPHATEST_ON");
            mat.EnableKeyword("_ALPHABLEND_ON");
            mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            mat.renderQueue = 3000;

            return mat;
        }

        #endregion

        #region 预览更新

        /// <summary>
        /// 更新预览
        /// </summary>
        private void UpdatePreview()
        {
            if (_editor == null || _editor.State == null || _editor.Grid == null)
            {
                HidePreview();
                return;
            }

            var state = _editor.State;
            var mode = state.CurrentMode;

            // 只在特定模式下显示预览
            if (mode != EditorMode.TileEdit && mode != EditorMode.ObjectPlace && mode != EditorMode.Delete)
            {
                HidePreview();
                return;
            }

            // 鼠标不在有效区域
            if (!state.IsMouseInValidArea)
            {
                HidePreview();
                return;
            }

            // 获取预览位置
            var positions = GetPreviewPositions(state, mode);
            
            if (positions.Count == 0)
            {
                HidePreview();
                return;
            }

            // 检查是否有效
            _isPreviewValid = CheckPreviewValid(positions, mode);

            // 更新预览网格
            UpdatePreviewMesh(positions);

            // 更新颜色
            UpdatePreviewColor(_isPreviewValid);

            // 显示预览
            _previewObject.SetActive(true);
        }

        /// <summary>
        /// 获取预览位置
        /// </summary>
        private List<TilePosition> GetPreviewPositions(RoomGridEditorState state, EditorMode mode)
        {
            _currentPreviewPositions.Clear();

            var mousePos = state.CurrentMouseTilePosition;
            int brushSize = state.BrushSize;

            if (mode == EditorMode.TileEdit)
            {
                var tool = state.CurrentTileTool;
                
                if (tool == TileEditTool.Brush || tool == TileEditTool.Eraser)
                {
                    // 画笔/橡皮擦 - 显示画笔范围
                    int halfSize = brushSize / 2;
                    
                    for (int dx = -halfSize; dx <= halfSize; dx++)
                    {
                        for (int dz = -halfSize; dz <= halfSize; dz++)
                        {
                            var pos = new TilePosition(mousePos.X + dx, mousePos.Z + dz);
                            if (_editor.Grid.Config.IsInBounds(pos))
                            {
                                _currentPreviewPositions.Add(pos);
                            }
                        }
                    }
                }
                else if (tool == TileEditTool.Rectangle && state.IsOperating)
                {
                    // 矩形工具 - 显示矩形范围
                    var startPos = state.DragStartPosition;
                    int minX = Mathf.Min(startPos.X, mousePos.X);
                    int maxX = Mathf.Max(startPos.X, mousePos.X);
                    int minZ = Mathf.Min(startPos.Z, mousePos.Z);
                    int maxZ = Mathf.Max(startPos.Z, mousePos.Z);

                    for (int x = minX; x <= maxX; x++)
                    {
                        for (int z = minZ; z <= maxZ; z++)
                        {
                            var pos = new TilePosition(x, z);
                            if (_editor.Grid.Config.IsInBounds(pos))
                            {
                                _currentPreviewPositions.Add(pos);
                            }
                        }
                    }
                }
                else
                {
                    // 单格预览
                    _currentPreviewPositions.Add(mousePos);
                }
            }
            else if (mode == EditorMode.ObjectPlace)
            {
                // 物品放置预览 - 根据物品尺寸
                var def = ObjectDefinitionManager.Instance.GetDefinition(state.SelectedObjectDefId);
                if (def != null)
                {
                    var size = def.Size.GetRotatedSize(state.CurrentRotation);
                    for (int dx = 0; dx < size.Width; dx++)
                    {
                        for (int dz = 0; dz < size.Depth; dz++)
                        {
                            var pos = new TilePosition(mousePos.X + dx, mousePos.Z + dz);
                            if (_editor.Grid.Config.IsInBounds(pos))
                            {
                                _currentPreviewPositions.Add(pos);
                            }
                        }
                    }
                }
                else
                {
                    // 没有定义时使用单格
                    _currentPreviewPositions.Add(mousePos);
                }
            }
            else if (mode == EditorMode.Delete)
            {
                // 删除预览
                _currentPreviewPositions.Add(mousePos);
            }

            return _currentPreviewPositions;
        }

        /// <summary>
        /// 检查预览是否有效
        /// </summary>
        private bool CheckPreviewValid(List<TilePosition> positions, EditorMode mode)
        {
            if (mode == EditorMode.TileEdit)
            {
                // 地块编辑总是有效
                return true;
            }
            else if (mode == EditorMode.ObjectPlace)
            {
                // 检查是否可以放置物品
                var def = ObjectDefinitionManager.Instance.GetDefinition(_editor.State.SelectedObjectDefId);
                var size = def?.Size ?? ObjectSize.One;
                
                // 使用Grid的检查方法
                if (positions.Count > 0)
                {
                    return _editor.Grid.CanPlaceObject(positions[0], size, _editor.State.CurrentRotation);
                }
                return false;
            }
            else if (mode == EditorMode.Delete)
            {
                // 检查是否有东西可删除
                foreach (var pos in positions)
                {
                    var tile = _editor.Grid.GetTile(pos);
                    if (tile != null && (tile.Type != TileType.None || tile.HasPlacedObject))
                    {
                        return true;
                    }
                }
                return false;
            }

            return true;
        }

        /// <summary>
        /// 更新预览网格
        /// </summary>
        private void UpdatePreviewMesh(List<TilePosition> positions)
        {
            if (positions.Count == 0)
            {
                _previewMesh.Clear();
                return;
            }

            var config = _editor.Grid.Config;
            float tileSize = config.TileSize;
            float halfSize = tileSize * 0.5f - _borderWidth;
            int heightLevel = _editor.State?.HeightLevel ?? 0;
            float height = heightLevel * 0.5f + _previewHeightOffset;

            var vertices = new List<Vector3>();
            var triangles = new List<int>();
            var uvs = new List<Vector2>();
            var colors = new List<Color>();

            foreach (var pos in positions)
            {
                Vector3 worldPos = config.TileToWorld(pos);
                int baseIndex = vertices.Count;

                // 四个顶点
                vertices.Add(new Vector3(worldPos.x - halfSize, height, worldPos.z - halfSize));
                vertices.Add(new Vector3(worldPos.x + halfSize, height, worldPos.z - halfSize));
                vertices.Add(new Vector3(worldPos.x + halfSize, height, worldPos.z + halfSize));
                vertices.Add(new Vector3(worldPos.x - halfSize, height, worldPos.z + halfSize));

                // UV
                uvs.Add(new Vector2(0, 0));
                uvs.Add(new Vector2(1, 0));
                uvs.Add(new Vector2(1, 1));
                uvs.Add(new Vector2(0, 1));

                // 颜色
                Color col = _isPreviewValid ? _validColor : _invalidColor;
                colors.Add(col);
                colors.Add(col);
                colors.Add(col);
                colors.Add(col);

                // 三角形
                triangles.Add(baseIndex + 0);
                triangles.Add(baseIndex + 2);
                triangles.Add(baseIndex + 1);
                triangles.Add(baseIndex + 0);
                triangles.Add(baseIndex + 3);
                triangles.Add(baseIndex + 2);
            }

            // 更新网格
            _previewMesh.Clear();
            _previewMesh.SetVertices(vertices);
            _previewMesh.SetTriangles(triangles, 0);
            _previewMesh.SetUVs(0, uvs);
            _previewMesh.SetColors(colors);
            _previewMesh.RecalculateNormals();
            _previewMesh.RecalculateBounds();
        }

        /// <summary>
        /// 更新预览颜色
        /// </summary>
        private void UpdatePreviewColor(bool valid)
        {
            if (_meshRenderer != null && _previewMaterial != null)
            {
                _previewMaterial.color = valid ? _validColor : _invalidColor;
            }
        }

        /// <summary>
        /// 隐藏预览
        /// </summary>
        public void HidePreview()
        {
            if (_previewObject != null)
            {
                _previewObject.SetActive(false);
            }
        }

        /// <summary>
        /// 显示预览
        /// </summary>
        public void ShowPreview()
        {
            if (_previewObject != null)
            {
                _previewObject.SetActive(true);
            }
        }

        #endregion

        #region 公共接口

        /// <summary>
        /// 设置预览颜色
        /// </summary>
        public void SetPreviewColors(Color validColor, Color invalidColor)
        {
            _validColor = validColor;
            _invalidColor = invalidColor;
        }

        /// <summary>
        /// 获取当前预览是否有效
        /// </summary>
        public bool IsPreviewValid()
        {
            return _isPreviewValid;
        }

        /// <summary>
        /// 获取当前预览位置
        /// </summary>
        public List<TilePosition> GetCurrentPreviewPositions()
        {
            return new List<TilePosition>(_currentPreviewPositions);
        }

        #endregion
    }
}