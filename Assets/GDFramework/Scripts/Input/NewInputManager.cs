using GDFramework_Core.Scripts.GDFrameworkCore;
using GDFramework_General.Models.Resource;

using UnityEngine;
using UnityEngine.InputSystem;

namespace GDFramework_Core.Input
{
    #region Mouse

    public struct SInputEvent_MouseDrag
    {
        public Vector2 mousePos;
    }

    public struct SInputEvent_MouseLeftClick
    {
    }

    public struct SInputEvent_MouseRightClick
    {
    }

    #endregion

    /// <summary>
    /// 新输入系统
    /// </summary>
    public class NewInputManager : AbstractSystem
    {
        private InputActionAsset actionAsset;

        #region Mouse

        private InputActionMap PlayerMouseMap;

        private InputAction mouseDrag;

        private InputAction mouseLeftClick;

        private InputAction mouseRightClick;

        private Vector2 curMousePosition;

        #endregion

        protected override void OnInit()
        {
        }

        protected override void OnDeinit()
        {
            UnregisterInputCallbacks();
            if (actionAsset != null) actionAsset.Disable();
        }

        /// <summary>
        /// 初始化输入
        /// </summary>
        public void InitActionAsset()
        {
            actionAsset = this.GetModel<LaunchResourcesDataModel>().InputActionAsset;

            PlayerMouseMap = actionAsset.FindActionMap("PlayerMouseMap");
            mouseDrag = actionAsset.FindAction("MouseDrag");
            mouseLeftClick = actionAsset.FindAction("MouseLeftClick");
            mouseRightClick = actionAsset.FindAction("MouseRightClick");

            CheckMouseMap(true);
            RegisterInputCallbacks();
        }

        public void CheckMouseMap(bool enable)
        {
            if (enable)
            {
                PlayerMouseMap.Enable();
                ;
            }
            else
            {
                PlayerMouseMap.Disable();
            }
        }

        /// <summary>
        /// 注册输入回调
        /// </summary>
        private void RegisterInputCallbacks()
        {
            if (mouseDrag != null)
            {
                mouseDrag.performed += HandleMouseDrag;
                mouseDrag.canceled += HandleMouseDrag_Cancel;
            }

            if (mouseLeftClick != null) mouseLeftClick.performed += HandleMouseLeftClick;

            if (mouseRightClick != null) mouseRightClick.performed += HandleMouseRightClick;
        }

        /// <summary>
        /// 注销输入回调
        /// </summary>
        private void UnregisterInputCallbacks()
        {
            if (mouseDrag != null)
            {
                mouseDrag.performed -= HandleMouseDrag;
                mouseDrag.canceled -= HandleMouseDrag_Cancel;
            }

            if (mouseLeftClick != null) mouseLeftClick.performed -= HandleMouseLeftClick;

            if (mouseRightClick != null) mouseRightClick.performed -= HandleMouseRightClick;
        }

        #region Mouse

        /// <summary>
        /// 是否显示鼠标在游戏内
        /// </summary>
        public void WillShowMouse()
        {
        }

        /// <summary>
        /// 是否限制鼠标在游戏内的移动(不让其超出操作界面)
        /// </summary>
        public void WillImposeMouse()
        {
        }

        /// <summary>
        /// 处理输入--鼠标移动（更新当前鼠标位置）
        /// </summary>
        /// <param name="context"></param>
        private void HandleMouseDrag(InputAction.CallbackContext context)
        {
            curMousePosition = context.ReadValue<Vector2>();
        }

        /// <summary>
        /// 清空鼠标输入（鼠标停止时）
        /// </summary>
        /// <param name="context"></param>
        private void HandleMouseDrag_Cancel(InputAction.CallbackContext context)
        {
            curMousePosition = Vector2.zero;
        }


        /// <summary>
        /// 处理输入--鼠标左键
        /// </summary>
        private void HandleMouseLeftClick(InputAction.CallbackContext context)
        {
            this.SendEvent<SInputEvent_MouseLeftClick>();
        }

        /// <summary>
        /// 处理输入--鼠标右键
        /// </summary>
        private void HandleMouseRightClick(InputAction.CallbackContext context)
        {
            this.SendEvent<SInputEvent_MouseRightClick>();
        }

        #endregion

        /// <summary>
        /// 改变输入键值
        /// </summary>
        public void ChangeInputKey()
        {
        }
    }
}