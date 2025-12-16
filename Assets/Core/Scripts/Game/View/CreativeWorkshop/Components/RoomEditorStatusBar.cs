using System;
using UnityEngine;
using UnityEngine.UI;
using Core.Game.Chunk.Room.Grid;

namespace Core.Game.View
{
    /// <summary>
    /// 房间编辑器状态栏
    /// 显示当前状态、统计信息和操作提示
    /// </summary>
    public class RoomEditorStatusBar : MonoBehaviour
    {
        #region UI引用

        private RectTransform _rectTransform;
        private HorizontalLayoutGroup _layoutGroup;

        // 状态文本
        private Text _statusText;

        // 楼层显示
        private Text _floorText;

        // 统计信息
        private Text _statisticsText;

        // 坐标显示
        private Text _positionText;

        // 帮助提示
        private Text _helpText;

        #endregion

        #region 配置

        private string _currentStatus = "就绪";
        private int _currentFloor = 0;
        private RoomGridStatistics _currentStats;

        #endregion

        #region 初始化

        public void Initialize()
        {
            _rectTransform = GetComponent<RectTransform>();
            if (_rectTransform == null)
            {
                _rectTransform = gameObject.AddComponent<RectTransform>();
            }

            SetupLayout();
            CreateStatusElements();

            Debug.Log("[RoomEditorStatusBar] 初始化完成");
        }

        private void SetupLayout()
        {
            // 设置为底部状态栏
            _rectTransform.anchorMin = new Vector2(0, 0);
            _rectTransform.anchorMax = new Vector2(1, 0);
            _rectTransform.pivot = new Vector2(0.5f, 0);
            _rectTransform.anchoredPosition = new Vector2(0, 0);
            _rectTransform.sizeDelta = new Vector2(0, 30);

            // 添加背景
            var bgImage = gameObject.AddComponent<Image>();
            bgImage.color = new Color(0.15f, 0.15f, 0.15f, 0.95f);

            // 添加水平布局
            _layoutGroup = gameObject.AddComponent<HorizontalLayoutGroup>();
            _layoutGroup.padding = new RectOffset(15, 15, 3, 3);
            _layoutGroup.spacing = 20;
            _layoutGroup.childAlignment = TextAnchor.MiddleLeft;
            _layoutGroup.childControlWidth = false;
            _layoutGroup.childControlHeight = true;
            _layoutGroup.childForceExpandWidth = false;
            _layoutGroup.childForceExpandHeight = true;
        }

        #endregion

        #region UI创建

        private void CreateStatusElements()
        {
            // 状态文本（左侧）
            _statusText = CreateStatusText("StatusText", "就绪", 200, TextAnchor.MiddleLeft);

            // 分隔符
            CreateSeparator();

            // 楼层显示
            _floorText = CreateStatusText("FloorText", "1F", 50, TextAnchor.MiddleCenter);

            // 分隔符
            CreateSeparator();

            // 统计信息
            _statisticsText = CreateStatusText("StatsText", "地块: 0 | 物品: 0", 200, TextAnchor.MiddleCenter);

            // 分隔符
            CreateSeparator();

            // 坐标显示
            _positionText = CreateStatusText("PositionText", "X: 0, Z: 0", 120, TextAnchor.MiddleCenter);

            // 弹性空间
            CreateFlexibleSpace();

            // 帮助提示（右侧）
            _helpText = CreateStatusText("HelpText", "按 1-5 切换模式 | Ctrl+S 保存", 300, TextAnchor.MiddleRight);
            _helpText.color = new Color(0.6f, 0.6f, 0.6f);
        }

        private Text CreateStatusText(string name, string text, float width, TextAnchor alignment)
        {
            var textGO = new GameObject(name, typeof(RectTransform));
            textGO.transform.SetParent(transform);

            var rect = textGO.GetComponent<RectTransform>();
            var layoutElement = textGO.AddComponent<LayoutElement>();
            layoutElement.preferredWidth = width;
            layoutElement.minWidth = 50;

            var textComp = textGO.AddComponent<Text>();
            textComp.text = text;
            textComp.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            textComp.fontSize = 12;
            textComp.color = Color.white;
            textComp.alignment = alignment;

            return textComp;
        }

        private void CreateSeparator()
        {
            var separator = new GameObject("Separator", typeof(RectTransform));
            separator.transform.SetParent(transform);

            var rect = separator.GetComponent<RectTransform>();
            var layoutElement = separator.AddComponent<LayoutElement>();
            layoutElement.preferredWidth = 1;
            layoutElement.minWidth = 1;

            var image = separator.AddComponent<Image>();
            image.color = new Color(0.4f, 0.4f, 0.4f);
        }

        private void CreateFlexibleSpace()
        {
            var space = new GameObject("FlexSpace", typeof(RectTransform));
            space.transform.SetParent(transform);

            var layoutElement = space.AddComponent<LayoutElement>();
            layoutElement.flexibleWidth = 1;
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 更新状态文本
        /// </summary>
        public void UpdateStatus(string status)
        {
            _currentStatus = status;
            if (_statusText != null)
            {
                _statusText.text = status;
            }
        }

        /// <summary>
        /// 更新楼层显示
        /// </summary>
        public void UpdateFloor(int floor)
        {
            _currentFloor = floor;
            if (_floorText != null)
            {
                _floorText.text = $"{floor + 1}F";
            }
        }

        /// <summary>
        /// 更新统计信息
        /// </summary>
        public void UpdateStatistics(RoomGridStatistics stats)
        {
            _currentStats = stats;
            if (_statisticsText != null)
            {
                _statisticsText.text = $"地块: {stats.TotalTiles} | 物品: {stats.PlacedObjects}";
            }
        }

        /// <summary>
        /// 更新坐标显示
        /// </summary>
        public void UpdatePosition(TilePosition pos)
        {
            if (_positionText != null)
            {
                _positionText.text = $"X: {pos.X}, Z: {pos.Z}";
            }
        }

        /// <summary>
        /// 更新坐标显示（世界坐标）
        /// </summary>
        public void UpdatePosition(Vector3 worldPos)
        {
            if (_positionText != null)
            {
                _positionText.text = $"X: {worldPos.x:F1}, Z: {worldPos.z:F1}";
            }
        }

        /// <summary>
        /// 更新帮助提示
        /// </summary>
        public void UpdateHelpText(string help)
        {
            if (_helpText != null)
            {
                _helpText.text = help;
            }
        }

        /// <summary>
        /// 根据编辑模式更新帮助提示
        /// </summary>
        public void UpdateHelpForMode(EditorMode mode)
        {
            string help = mode switch
            {
                EditorMode.View => "拖拽查看 | 滚轮缩放",
                EditorMode.TileEdit => "左键绘制 | Q/W/E 切换工具 | [/] 画笔大小",
                EditorMode.ObjectPlace => "左键放置 | R 旋转 | 右键取消",
                EditorMode.ObjectSelect => "左键选择 | R 旋转 | Delete 删除",
                EditorMode.Delete => "左键删除 | 右键取消",
                _ => "按 1-5 切换模式 | Ctrl+S 保存"
            };

            UpdateHelpText(help);
        }

        /// <summary>
        /// 显示临时消息（几秒后恢复）
        /// </summary>
        public void ShowTemporaryMessage(string message, float duration = 3f)
        {
            UpdateStatus(message);
            CancelInvoke(nameof(RestoreStatus));
            Invoke(nameof(RestoreStatus), duration);
        }

        private void RestoreStatus()
        {
            UpdateStatus("就绪");
        }

        #endregion
    }
}
