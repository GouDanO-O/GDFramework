using Core.Game.Chunk.Data;
using Core.Game.View.Interface;
using GDFrameworkCore;
using GDFrameworkExtend.FluentAPI;
using GDFrameworkExtend.UIKit;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Core.Game.View.Details
{
    /// <summary>
    /// 地图节点基类
    /// TDef: 该节点对应的 DtoDef 类型
    /// </summary>
    public abstract class UI_EditorDetail_MapNode<TDef> : UI_Details, IUIViewDraggable, ICanGetSystem
        where TDef : ChunkDtoDef
    {
        #region 拖拽相关字段

        [Header("拖拽设置")]
        [SerializeField] protected float dragSmoothness = 0.1f;
        [SerializeField] protected float dragAlpha = 0.6f;

        protected RectTransform rectTransform;
        protected Canvas canvas;
        protected CanvasGroup canvasGroup;

        protected bool isDragging = false;
        protected Vector2 originalPosition;
        protected Vector2 dragOffset;
        protected Vector2 targetPosition;

        protected float pointerDownTime;
        protected Vector2 pointerDownPosition;
        protected const float clickThreshold = 0.2f;
        protected const float clickDistanceThreshold = 5f;

        #endregion

        #region UI组件

        protected TextMeshProUGUI NodeName;
        protected TextMeshProUGUI ChangeLockDes;
        protected TextMeshProUGUI ChangeInitialPlayerLocateNodeDes;
        protected GameObject SelectingOutline;
        protected GameObject NodeUnlockImage;
        protected GameObject NodeLockImage;
        protected Transform DownerButtons;
        protected Button SetInitialPlayerLocateButton;
        protected Button CopyThisButton;
        protected Button ChangeLockButton;
        protected Button DeleteThisButton;
        protected Button ShowDetailButton;

        #endregion

        #region 状态数据

        protected TDef dtoDef;
        protected bool ThisNodeIsInitial = false;
        protected bool ThisNodeIsLocking = false;
        protected EditorDataManager editorDataManager;

        #endregion

        public IArchitecture GetArchitecture()
        {
            return GameMain.Interface;
        }

        protected override void OnInit()
        {
            InitializeDragComponents();
            editorDataManager = this.GetSystem<EditorDataManager>();
        }

        protected override void OnShow() { }
        protected override void OnStart() { }
        protected override void OnClose() { }

        #region 初始化

        protected virtual void InitializeDragComponents()
        {
            rectTransform = GetComponent<RectTransform>();
            canvas = GetComponentInParent<Canvas>();

            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }

            originalPosition = rectTransform.anchoredPosition;
            targetPosition = originalPosition;

            SelectingOutline = transform.Find("SelectingOutline").gameObject;
            NodeName = transform.Find("NodeName").GetComponent<TextMeshProUGUI>();

            DownerButtons = transform.Find("DownerButtons");
            SetInitialPlayerLocateButton = DownerButtons.Find("SetInitialPlayerLocateButton")
                .GetComponent<Button>();
            ChangeInitialPlayerLocateNodeDes = SetInitialPlayerLocateButton.transform.GetChild(0)
                .GetComponent<TextMeshProUGUI>();

            CopyThisButton = DownerButtons.Find("CopyThisButton").GetComponent<Button>();
            ChangeLockButton = DownerButtons.Find("ChangeLockButton").GetComponent<Button>();
            DeleteThisButton = DownerButtons.Find("DeleteThisButton").GetComponent<Button>();

            ChangeLockDes = ChangeLockButton.transform.Find("ChangeLockDes")
                .GetComponent<TextMeshProUGUI>();
            NodeUnlockImage = ChangeLockButton.transform.Find("LockImage/Unlock").gameObject;
            NodeLockImage = ChangeLockButton.transform.Find("LockImage/Lock").gameObject;

            ShowDetailButton = transform.Find("ShowDetailButton").GetComponent<Button>();

            SetInitialPlayerLocateButton.onClick.AddListener(SetThisNodeAsInitial);
            CopyThisButton.onClick.AddListener(CopyThisNode);
            DeleteThisButton.onClick.AddListener(DeleteThisNode);
            ChangeLockButton.onClick.AddListener(ChangeWillLockThisNode);
            ShowDetailButton.onClick.AddListener(ShowNodeDetail);
        }

        /// <summary>
        /// 设置节点数据
        /// </summary>
        public virtual void SetDto<TMap>(TMap map, TDef curDtoDef)
            where TMap : UI_Details
        {
            dtoDef = curDtoDef;
            LoadNodePosition();
            UpdateNodeName();
            OnDataSet(map);
        }

        /// <summary>
        /// 数据设置完成后的回调
        /// </summary>
        protected virtual void OnDataSet<TMap>(TMap map) where TMap : UI_Details { }

        #endregion

        #region 数据访问

        /// <summary>
        /// 获取当前节点的 DtoDef
        /// </summary>
        public TDef GetDtoDef() => dtoDef;

        /// <summary>
        /// 获取节点锁定状态
        /// </summary>
        public bool GetThisNodeIsLocking() => ThisNodeIsLocking;

        #endregion

        #region 节点状态管理

        /// <summary>
        /// 改变选中状态
        /// </summary>
        public virtual void ChangeSelectingNode(bool isSelecting)
        {
            SelectingOutline.SetActive(isSelecting);
            DownerButtons.gameObject.SetActive(isSelecting);
            ShowDetailButton.gameObject.SetActive(isSelecting);

            if (isSelecting)
            {
                OnNodeSelected();
                if (!ThisNodeIsInitial)
                {
                    SetInitialPlayerLocateButton.gameObject.Show();
                }
            }
            else
            {
                SetInitialPlayerLocateButton.gameObject.Hide();
            }
        }

        /// <summary>
        /// 节点被选中时的回调
        /// </summary>
        protected virtual void OnNodeSelected() { }

        /// <summary>
        /// 改变初始节点状态
        /// </summary>
        public virtual void ChangeInitialNode(bool isInitial)
        {
            ThisNodeIsInitial = isInitial;
            UpdateInitialNodeUI(isInitial);
        }

        /// <summary>
        /// 更新初始节点UI
        /// </summary>
        protected abstract void UpdateInitialNodeUI(bool isInitial);

        /// <summary>
        /// 设置为初始节点
        /// </summary>
        public virtual void SetThisNodeAsInitial()
        {
            ChangeInitialNode(true);
            ChangeSelectingNode(true);
        }

        /// <summary>
        /// 设置节点是否为锁定状态
        /// </summary>
        public virtual void SetThisNodeWillLock(bool isLock)
        {
            ThisNodeIsLocking = isLock;
            UpdateLockUI();
        }

        /// <summary>
        /// 改变锁定状态
        /// </summary>
        protected virtual void ChangeWillLockThisNode()
        {
            ThisNodeIsLocking = !ThisNodeIsLocking;
            UpdateLockUI();
            OnLockStateChanged(ThisNodeIsLocking);
        }

        /// <summary>
        /// 更新锁定状态UI
        /// </summary>
        protected virtual void UpdateLockUI()
        {
            if (ThisNodeIsLocking)
            {
                ChangeLockDes.text = "锁定";
                NodeUnlockImage.Hide();
                NodeLockImage.Show();
            }
            else
            {
                ChangeLockDes.text = "解锁";
                NodeUnlockImage.Show();
                NodeLockImage.Hide();
            }
        }

        /// <summary>
        /// 锁定状态改变时的回调
        /// </summary>
        protected virtual void OnLockStateChanged(bool isLocked) { }

        #endregion

        #region 抽象方法

        /// <summary>
        /// 显示节点详情
        /// </summary>
        protected abstract void ShowNodeDetail();

        /// <summary>
        /// 更新节点名称显示
        /// </summary>
        protected virtual void UpdateNodeName()
        {
            NodeName.text = dtoDef.DefName;
        }

        /// <summary>
        /// 加载节点位置
        /// </summary>
        protected abstract void LoadNodePosition();

        /// <summary>
        /// 保存节点位置
        /// </summary>
        protected abstract void SaveNodePosition(Vector2 newPos);

        #endregion

        #region 节点操作

        /// <summary>
        /// 复制节点
        /// </summary>
        protected virtual void CopyThisNode()
        {
            // TODO: 实现复制逻辑
        }

        /// <summary>
        /// 删除节点
        /// </summary>
        protected virtual void DeleteThisNode()
        {
            // TODO: 实现删除逻辑
        }

        /// <summary>
        /// 销毁节点
        /// </summary>
        public void SetDestroy()
        {
            Destroy(gameObject);
        }

        #endregion

        #region 拖拽实现

        protected virtual void OnDragStart() { }
        protected virtual void OnDragging() { }

        protected virtual void OnDragEnd(Vector2 screenPosition)
        {
            SaveNodePosition(screenPosition);
        }

        protected void ClickButtonCheck()
        {
            ChangeSelectingNode(true);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            pointerDownTime = Time.time;
            pointerDownPosition = eventData.position;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            float holdTime = Time.time - pointerDownTime;
            float dragDistance = Vector2.Distance(pointerDownPosition, eventData.position);

            if (holdTime < clickThreshold && dragDistance < clickDistanceThreshold && !isDragging)
            {
                ClickButtonCheck();
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            isDragging = true;
            originalPosition = rectTransform.anchoredPosition;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform.parent as RectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out Vector2 localPoint
            );

            dragOffset = rectTransform.anchoredPosition - localPoint;

            if (canvasGroup != null)
            {
                canvasGroup.alpha = dragAlpha;
            }

            OnDragStart();
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!isDragging) return;

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    rectTransform.parent as RectTransform,
                    eventData.position,
                    eventData.pressEventCamera,
                    out Vector2 localPoint))
            {
                targetPosition = localPoint + dragOffset;
                targetPosition = ClampToParentBounds(targetPosition);

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

            OnDragging();
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            isDragging = false;

            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f;
            }

            originalPosition = rectTransform.anchoredPosition;
            OnDragEnd(originalPosition);
        }

        protected Vector2 ClampToParentBounds(Vector2 position)
        {
            if (rectTransform.parent == null) return position;

            RectTransform parentRect = rectTransform.parent as RectTransform;
            if (parentRect == null) return position;

            Vector2 parentSize = parentRect.rect.size;
            Vector2 nodeSize = rectTransform.rect.size;

            float minX = -parentSize.x / 2 + nodeSize.x / 2;
            float maxX = parentSize.x / 2 - nodeSize.x / 2;
            float minY = -parentSize.y / 2 + nodeSize.y / 2;
            float maxY = parentSize.y / 2 - nodeSize.y / 2;

            position.x = Mathf.Clamp(position.x, minX, maxX);
            position.y = Mathf.Clamp(position.y, minY, maxY);

            return position;
        }

        public void ResetPosition()
        {
            rectTransform.anchoredPosition = originalPosition;
        }

        public void SetNodePosition(Vector2 position)
        {
            rectTransform.anchoredPosition = position;
            originalPosition = position;
            targetPosition = position;
        }

        public Vector2 GetNodePosition()
        {
            return rectTransform.anchoredPosition;
        }

        #endregion
    }
}