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
    public class UI_EditorDetail_UniverseMapWorldNode : UI_Details, IBeginDragHandler, IDragHandler, IEndDragHandler,
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

        private UI_EditorDetail_UniverseMap _universeMap;

        protected TextMeshProUGUI WorldName;

        protected TextMeshProUGUI ChangeWorldLockDes;

        protected TextMeshProUGUI ChangeInitialPlayerLocateWorldDes;
        
        protected GameObject SelectingOutline;

        protected GameObject WorldUnlockImage;

        protected GameObject WorldLockImage;

        protected Transform DownerButtons;

        protected Button SetInitialPlayerLocateWorldButton;

        protected Button CopyThisWorldButton;
        
        protected Button ChangeWorldLockButton;
        
        protected Button DeleteThisWorldButton;

        protected Button ShowDetailButton;
        
        private bool _thisWorldIsInitialWorld = false;

        private bool _thisWorldIsLocking = false;
        
        private WorldDtoDef _worldDto;

        #region 初始化

        protected override void OnInit()
        {
            InitializeDragComponents();
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

        /// <summary>
        /// 设置世界固定数据
        /// </summary>
        /// <param name="worldDto"></param>
        public void SetWorldDto(UI_EditorDetail_UniverseMap universeMap, WorldDtoDef worldDto)
        {
            _universeMap = universeMap;
            _worldDto = worldDto;
            SetWorldPosition(worldDto.InitialSpawnedPosition);
            UpdateWorldName();
        }

        private void InitializeDragComponents()
        {
            rectTransform = GetComponent<RectTransform>();
            canvas = GetComponentInParent<Canvas>();
            
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
            
            SelectingOutline = transform.Find("SelectingOutline").gameObject;

            WorldName = transform.Find("WorldName").GetComponent<TextMeshProUGUI>();
            
            DownerButtons = transform.Find("DownerButtons");
            SetInitialPlayerLocateWorldButton = DownerButtons.Find("SetInitialPlayerLocateWorldButton")
                .GetComponent<Button>();

            ChangeInitialPlayerLocateWorldDes = SetInitialPlayerLocateWorldButton.transform.GetChild(0)
                .GetComponent<TextMeshProUGUI>();
            
            CopyThisWorldButton = DownerButtons.Find("CopyThisWorldButton").GetComponent<Button>();
            ChangeWorldLockButton = DownerButtons.Find("ChangeWorldLockButton").GetComponent<Button>();
            
            DeleteThisWorldButton = DownerButtons.Find("DeleteThisWorldButton").GetComponent<Button>();

            ChangeWorldLockDes = ChangeWorldLockButton.transform.Find("ChangeWorldLockDes")
                .GetComponent<TextMeshProUGUI>();
            WorldUnlockImage = ChangeWorldLockButton.transform.Find("LockImage/Unlock").gameObject;
            WorldLockImage = ChangeWorldLockButton.transform.Find("LockImage/Lock").gameObject;


            ShowDetailButton = transform.Find("ShowDetailButton").GetComponent<Button>();
            
            // 保存初始位置
            originalPosition = rectTransform.anchoredPosition;
            targetPosition = originalPosition;


            SetInitialPlayerLocateWorldButton.onClick.AddListener(SetThisWorldAsInitialWorld);
            CopyThisWorldButton.onClick.AddListener(CopyThisWorld);
            DeleteThisWorldButton.onClick.AddListener(DeleteThisWorld);
            ChangeWorldLockButton.onClick.AddListener(ChangeWillLockThisWorld);
            
            ShowDetailButton.onClick.AddListener(ShowWorldDetail);
        }

        #endregion

        public void SetDestroy()
        {
            Destroy(gameObject);
        }

        public WorldDtoDef GetThisWorldDtoDef()
        {
            return _worldDto;
        }

        /// <summary>
        /// 展示当前的世界详情
        /// </summary>
        private void ShowWorldDetail()
        {
            UIKit.GetPanel<UI_Editor_UniversePanel>().OpenWorldDetail(_worldDto);
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
        protected void OnDragEnd(Vector2 screenPosition)
        {
            SaveWorldPosition(screenPosition);
        }

        /// <summary>
        /// 点击世界节点
        /// </summary>
        protected void ClickButtonCheck()
        {
            ChangeSelecting(true);
        }

        /// <summary>
        /// 切换该世界的锁定状态
        /// 即初次进入宇宙,当前世界是否需要条件才能解锁
        /// </summary>
        private void ChangeWillLockThisWorld()
        {
            _thisWorldIsLocking = !_thisWorldIsLocking;
            _worldDto.IsLockInInitialUniverse = _thisWorldIsLocking;
            if (_thisWorldIsLocking)
            {
                ChangeWorldLockDes.text = "锁定";
                WorldUnlockImage.Hide();
                WorldLockImage.Show();
            }
            else
            {
                ChangeWorldLockDes.text = "解锁";
                WorldUnlockImage.Show();
                WorldLockImage.Hide();
            }
        }

        /// <summary>
        /// 当前宇宙是否解锁
        /// </summary>
        /// <returns></returns>
        public bool GetThisWorldIsLocking()
        {
            return _thisWorldIsLocking;
        }

        /// <summary>
        /// 切换是否选择中
        /// </summary>
        /// <param name="isSelecting"></param>
        public void ChangeSelecting(bool isSelecting)
        {
            if (isSelecting)
            {
                _universeMap.ManageWorldSelect(this);
                if (ThisWorldIsInitialWorld())
                {
                }
                else
                {
                    SetInitialPlayerLocateWorldButton.gameObject.Show();
                }
            }
            else
            {
                SetInitialPlayerLocateWorldButton.gameObject.Hide();
            }

            SelectingOutline.SetActive(isSelecting);
            DownerButtons.gameObject.SetActive(isSelecting);
            ShowDetailButton.gameObject.SetActive(isSelecting);
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
        public void ChangeInitialWorld(bool isInitialWorld)
        {
            _thisWorldIsInitialWorld = isInitialWorld;
            if (isInitialWorld)
            {
                ChangeInitialPlayerLocateWorldDes.text = "初始世界";
            }
            else
            {
                ChangeInitialPlayerLocateWorldDes.text = "设置为初始世界";
            }
        }

        /// <summary>
        /// 当前世界是否是初始世界
        /// </summary>
        /// <returns></returns>
        private bool ThisWorldIsInitialWorld()
        {
            return _thisWorldIsInitialWorld;
        }

        /// <summary>
        /// 将当前世界设置为初始世界
        /// </summary>
        public void SetThisWorldAsInitialWorld()
        {
            ChangeInitialWorld(true);
            ChangeSelecting(true);
            _universeMap.UpdateInitialWorld(this);

        }

        /// <summary>
        /// 更改世界名称
        /// </summary>
        protected void UpdateWorldName()
        {
            WorldName.text = _worldDto.DefName;
        }

        /// <summary>
        /// 保存世界位置
        /// </summary>
        protected void SaveWorldPosition(Vector2 newPos)
        {
            _worldDto.InitialSpawnedPosition = newPos;
        }

        /// <summary>
        /// 删除当前世界
        /// </summary>
        protected void DeleteThisWorld()
        {
            
        }


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
                ClickButtonCheck();
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