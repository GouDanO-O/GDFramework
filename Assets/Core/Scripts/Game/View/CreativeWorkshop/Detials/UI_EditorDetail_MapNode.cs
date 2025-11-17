using Core.Game.Chunk.Data.Interface;
using Core.Game.View.Details.Interface;
using GDFrameworkExtend.FluentAPI;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Core.Game.View.Details
{
    public class UI_EditorDetail_MapNode  : UI_Details,IUI_EditorDetail_MapNode, IBeginDragHandler, IDragHandler, IEndDragHandler,
        IPointerDownHandler, IPointerUpHandler
    {
        #region 拖拽相关字段

        [Header("拖拽设置")]
        [SerializeField]
        private float dragSmoothness = 0.1f; // 拖拽平滑度 (0-1)

        [SerializeField]
        private float dragAlpha = 0.6f;

        private RectTransform rectTransform;
        private Canvas canvas;
        private CanvasGroup canvasGroup;

        // 拖拽状态
        private bool isDragging = false;
        private Vector2 originalPosition;
        private Vector2 dragOffset;
        private Vector2 targetPosition;

        // 点击检测
        private float pointerDownTime;
        private Vector2 pointerDownPosition;
        private const float clickThreshold = 0.2f; // 点击时间阈值
        private const float clickDistanceThreshold = 5f; // 点击距离阈值

        #endregion
        
        protected IChunkDtoDef _thisChunkDtoDef;

        protected UI_EditorDetail_Map _editorDetailMap;
        
        protected bool ThisNodeIsLocking = false;
        
        
        protected TextMeshProUGUI NodeName;

        protected TextMeshProUGUI ChangeNodeLockDes;
        
        protected TextMeshProUGUI ChangeInitialPlayerLocateDes;
        
        protected GameObject SelectingOutline;

        protected GameObject WorldUnlockImage;

        protected GameObject WorldLockImage;
        
        protected Transform DownerButtons;
        
        protected Button ShowDetailButton;
        
        protected Button SetInitialPlayerLocateNodeButton;

        protected Button CopyThisNodeButton;
        
        protected Button ChangeNodeLockButton;
        
        protected Button DeleteThisNodeButton;
        
        protected override void OnInit()
        {
            InitializedComponents();
        }

        protected override void OnShow()
        {
            
        }

        protected override void OnStart()
        {
            
        }

        protected override void OnClose()
        {
            
        }

        protected virtual void InitializedComponents()
        {
                        rectTransform = GetComponent<RectTransform>();
            canvas = GetComponentInParent<Canvas>();
            
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
            
            SelectingOutline = transform.Find("SelectingOutline").gameObject;

            NodeName = transform.Find("NodeName").GetComponent<TextMeshProUGUI>();
            
            DownerButtons = transform.Find("DownerButtons");
            SetInitialPlayerLocateNodeButton = DownerButtons.Find("SetInitialPlayerLocateNodeButton")
                .GetComponent<Button>();

            ChangeInitialPlayerLocateDes = SetInitialPlayerLocateNodeButton.transform.GetChild(0)
                .GetComponent<TextMeshProUGUI>();
            
            CopyThisNodeButton = DownerButtons.Find("CopyThisNodeButton").GetComponent<Button>();
            ChangeNodeLockButton = DownerButtons.Find("ChangeNodeLockButton").GetComponent<Button>();
            
            DeleteThisNodeButton = DownerButtons.Find("DeleteThisNodeButton").GetComponent<Button>();

            ChangeNodeLockDes = ChangeNodeLockButton.transform.Find("ChangeNodeLockDes")
                .GetComponent<TextMeshProUGUI>();
            WorldUnlockImage = ChangeNodeLockButton.transform.Find("LockImage/Unlock").gameObject;
            WorldLockImage = ChangeNodeLockButton.transform.Find("LockImage/Lock").gameObject;


            ShowDetailButton = transform.Find("ShowDetailButton").GetComponent<Button>();
            
            // 保存初始位置
            originalPosition = rectTransform.anchoredPosition;
            targetPosition = originalPosition;


            // SetInitialPlayerLocateWorldButton.onClick.AddListener(SetThisWorldAsInitialWorld);
            // CopyThisNodeButton.onClick.AddListener(CopyThisWorld);
            // DeleteThisNodeButton.onClick.AddListener(DeleteThisWorld);
            // DeleteThisNodeButton.onClick.AddListener(ChangeWillLockThisWorld);
            //
            // ShowDetailButton.onClick.AddListener(ShowWorldDetail);
        }
        
        /// <summary>
        /// 拖拽开始时调用
        /// </summary>
        protected void OnDragStart()
        {
            
        }

        /// <summary>
        /// 拖拽中调用
        /// </summary>
        protected void OnDragging()
        {
            
        }

        /// <summary>
        /// 拖拽结束时调用
        /// </summary>
        protected virtual void OnDragEnd(Vector2 screenPosition)
        {
            
        }

        public void SetDestroy()
        {
            
        }

        /// <summary>
        /// 设置当前节点的配置
        /// </summary>
        /// <param name="map"></param>
        /// <param name="nodeDef"></param>
        public virtual void SetMapNodeDto(UI_EditorDetail_Map map, IChunkDtoDef nodeDef)
        {
            _editorDetailMap = map;
            _thisChunkDtoDef = nodeDef;
        }
        
        public IChunkDtoDef GetThisNodeDtoDef()
        {
            return _thisChunkDtoDef;
        }

        public bool GetThisNodeIsLocking()
        {
            return ThisNodeIsLocking;
        }

        #region Control

        protected virtual void ClickThisNode()
        {
            ChangeSelecting(true);
        }
        
        protected virtual void ChangeWillLockThisNode()
        {
            ThisNodeIsLocking = !ThisNodeIsLocking;
            _thisChunkDtoDef.IsLockInInitial = ThisNodeIsLocking;
        }

        public virtual void ChangeSelecting(bool isSelecting)
        {
            if (isSelecting)
            {
                _editorDetailMap.ManageMapNodeSelect(this);
                if (ThisNodeIsInitial())
                {
                    
                }
                else
                {
                    SetInitialPlayerLocateNodeButton.gameObject.Show();
                }
            }
            else
            {
                SetInitialPlayerLocateNodeButton.gameObject.Hide();
            }

            SelectingOutline.SetActive(isSelecting);
            DownerButtons.gameObject.SetActive(isSelecting);
            ShowDetailButton.gameObject.SetActive(isSelecting);
        }
        

        public virtual void ChangeInitialNode(bool isInitialNode)
        {

        }

        protected bool ThisNodeIsInitial()
        {
            return true;
        }

        #endregion

        #region 拖拽事件处理

        public void OnPointerDown(PointerEventData eventData)
        {
            pointerDownTime = Time.time;
            pointerDownPosition = eventData.position;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            // 判断是否为点击操作（而非拖拽）
            float holdTime = Time.time - pointerDownTime;
            float dragDistance = Vector2.Distance(pointerDownPosition, eventData.position);

            if (holdTime < clickThreshold && dragDistance < clickDistanceThreshold && !isDragging)
            {
                ClickThisNode();
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            isDragging = true;

            // 保存原始位置
            originalPosition = rectTransform.anchoredPosition;

            // 计算拖拽偏移
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform.parent as RectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out Vector2 localPoint
            );

            dragOffset = rectTransform.anchoredPosition - localPoint;

            // 视觉反馈：降低透明度
            if (canvasGroup != null)
            {
                canvasGroup.alpha = dragAlpha;
            }

            // 触发拖拽开始事件
            OnDragStart();
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!isDragging) return;

            // 将屏幕坐标转换为局部坐标
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    rectTransform.parent as RectTransform,
                    eventData.position,
                    eventData.pressEventCamera,
                    out Vector2 localPoint))
            {
                // 应用拖拽偏移
                targetPosition = localPoint + dragOffset;

                // 可选：限制拖拽范围
                targetPosition = ClampToParentBounds(targetPosition);

                // 平滑移动或直接移动
                if (dragSmoothness > 0)
                {
                    rectTransform.anchoredPosition = Vector2.Lerp(
                        rectTransform.anchoredPosition,
                        targetPosition,
                        1f - dragSmoothness
                    );
                }
                else
                {
                    rectTransform.anchoredPosition = targetPosition;
                }
            }

            // 触发拖拽中事件
            OnDragging();
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            isDragging = false;

            // 恢复透明度
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f;
            }

            originalPosition = rectTransform.anchoredPosition;

            // 触发拖拽结束事件
            OnDragEnd(originalPosition);
        }

        /// <summary>
        /// 限制位置在父容器范围内
        /// </summary>
        private Vector2 ClampToParentBounds(Vector2 position)
        {
            if (rectTransform.parent == null) return position;

            RectTransform parentRect = rectTransform.parent as RectTransform;
            if (parentRect == null) return position;

            // 获取父容器的尺寸
            Vector2 parentSize = parentRect.rect.size;
            Vector2 nodeSize = rectTransform.rect.size;

            // 计算边界
            float minX = -parentSize.x / 2 + nodeSize.x / 2;
            float maxX = parentSize.x / 2 - nodeSize.x / 2;
            float minY = -parentSize.y / 2 + nodeSize.y / 2;
            float maxY = parentSize.y / 2 - nodeSize.y / 2;

            // 限制位置
            position.x = Mathf.Clamp(position.x, minX, maxX);
            position.y = Mathf.Clamp(position.y, minY, maxY);

            return position;
        }

        /// <summary>
        /// 重置到原始位置
        /// </summary>
        public void ResetWorldPosition()
        {
            rectTransform.anchoredPosition = originalPosition;
        }

        /// <summary>
        /// 设置位置
        /// </summary>
        public void SetWorldPosition(Vector2 position)
        {
            rectTransform.anchoredPosition = position;
            originalPosition = position;
            targetPosition = position;
        }

        /// <summary>
        /// 获取当前位置
        /// </summary>
        public Vector2 GetWorldPosition()
        {
            return rectTransform.anchoredPosition;
        }

        #endregion
    }
}