using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace GDFrameworkExtend.UIKit
{
    /// <summary>
    /// 扩展的 TMP_InputField
    /// 1. 根据文本长度自适应高度
    /// 2. 限制最大输入字符数
    /// </summary>
    public class TMP_InputFieldExtended : TMP_InputField
    {
        [Header("高度自适应设置")]
        [SerializeField]
        private bool enableAutoHeight = true;

        [SerializeField]
        private float minHeight = 50f;

        [SerializeField]
        private float maxHeight = 300f;

        [SerializeField]
        private float paddingVertical = 10f;

        [Header("字符限制设置")]
        [SerializeField]
        private bool enableCharacterLimit = true;

        [SerializeField]
        private int maxCharacterCount = 500;

        [SerializeField]
        private bool showCharacterCounter = true;

        [SerializeField]
        private TMP_Text counterText; // 可选:显示字符计数的Text组件

        private RectTransform rectTransform;
        private LayoutElement layoutElement;
        private float lastHeight;

        protected override void Awake()
        {
            base.Awake();

            rectTransform = GetComponent<RectTransform>();

            // 添加LayoutElement组件用于控制高度
            if (enableAutoHeight)
            {
                layoutElement = GetComponent<LayoutElement>();
                if (layoutElement == null)
                {
                    layoutElement = gameObject.AddComponent<LayoutElement>();
                }

                layoutElement.preferredHeight = minHeight;
            }

            // 设置字符限制
            if (enableCharacterLimit)
            {
                characterLimit = maxCharacterCount;
            }
        }

        protected override void Start()
        {
            base.Start();

            // 注册事件监听
            onValueChanged.AddListener(OnInputValueChanged);

            // 初始化高度
            if (enableAutoHeight)
            {
                UpdateHeight();
            }

            // 初始化计数器
            if (showCharacterCounter && counterText != null)
            {
                UpdateCharacterCounter(text);
            }
        }

        protected override void OnDestroy()
        {
            onValueChanged.RemoveListener(OnInputValueChanged);
            base.OnDestroy();
        }

        /// <summary>
        /// 重写OnValidate,在编辑器中修改参数时自动更新
        /// </summary>
        protected override void OnValidate()
        {
            base.OnValidate();

            if (Application.isPlaying)
            {
                if (enableCharacterLimit)
                {
                    characterLimit = maxCharacterCount;
                }

                if (enableAutoHeight)
                {
                    UpdateHeight();
                }
            }
        }

        /// <summary>
        /// 输入框内容改变时调用
        /// </summary>
        public void OnInputValueChanged(string inputText)
        {
            if (enableAutoHeight)
            {
                UpdateHeight();
            }

            if (showCharacterCounter && counterText != null)
            {
                UpdateCharacterCounter(inputText);
            }
        }

        /// <summary>
        /// 更新输入框高度
        /// </summary>
        private void UpdateHeight()
        {
            if (textComponent == null) return;

            // 强制更新文本布局
            Canvas.ForceUpdateCanvases();
            textComponent.ForceMeshUpdate();

            // 计算文本的preferredHeight
            float textHeight = textComponent.preferredHeight;

            // 添加padding并限制在最小和最大高度之间
            float newHeight = Mathf.Clamp(textHeight + paddingVertical * 2, minHeight, maxHeight);

            // 只有高度变化时才更新,避免不必要的重绘
            if (Mathf.Abs(newHeight - lastHeight) > 0.1f)
            {
                lastHeight = newHeight;
                if (layoutElement != null)
                {
                    layoutElement.preferredHeight = newHeight;
                }
                else
                {
                    rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newHeight);
                }
            }
        }

        /// <summary>
        /// 更新字符计数显示
        /// </summary>
        private void UpdateCharacterCounter(string inputText)
        {
            int currentLength = inputText.Length;
            counterText.text = $"{currentLength}/{maxCharacterCount}";

            // 可选:当接近上限时改变颜色
            if (currentLength >= maxCharacterCount * 0.9f)
            {
                counterText.color = Color.red;
            }
            else if (currentLength >= maxCharacterCount * 0.7f)
            {
                counterText.color = Color.yellow;
            }
            else
            {
                counterText.color = Color.white;
            }
        }

        #region 公共方法

        /// <summary>
        /// 手动刷新高度(在运行时修改设置后调用)
        /// </summary>
        public void RefreshHeight()
        {
            if (enableAutoHeight)
            {
                UpdateHeight();
            }
        }

        /// <summary>
        /// 设置最大字符数
        /// </summary>
        public void SetMaxCharacterCount(int count)
        {
            maxCharacterCount = count;
            if (enableCharacterLimit)
            {
                characterLimit = maxCharacterCount;
            }

            if (showCharacterCounter && counterText != null)
            {
                UpdateCharacterCounter(text);
            }
        }

        /// <summary>
        /// 设置高度范围
        /// </summary>
        public void SetHeightRange(float min, float max)
        {
            minHeight = min;
            maxHeight = max;
            RefreshHeight();
        }

        /// <summary>
        /// 启用/禁用自适应高度
        /// </summary>
        public void SetAutoHeightEnabled(bool enabled)
        {
            enableAutoHeight = enabled;
            if (enabled)
            {
                if (layoutElement == null)
                {
                    layoutElement = gameObject.AddComponent<LayoutElement>();
                }

                UpdateHeight();
            }
        }

        /// <summary>
        /// 启用/禁用字符限制
        /// </summary>
        public void SetCharacterLimitEnabled(bool enabled)
        {
            enableCharacterLimit = enabled;
            characterLimit = enabled ? maxCharacterCount : 0;
        }

        /// <summary>
        /// 获取当前字符数
        /// </summary>
        public int GetCurrentCharacterCount()
        {
            return text.Length;
        }

        /// <summary>
        /// 获取剩余可输入字符数
        /// </summary>
        public int GetRemainingCharacterCount()
        {
            return maxCharacterCount - text.Length;
        }

        #endregion
    }
}