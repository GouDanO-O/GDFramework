using GDFrameworkCore;

namespace GDFramework.World.Object
{ 
    public interface IObjMovement
    {
        void InitMovement();
        
        #region Move

        BindableProperty<float> CurMoveSpeed { get; set; }
        
        BindableProperty<float> MoveSpeed { get; set; }
        
        bool Runable { get; set; }
        
        BindableProperty<float> RunningSpeed { get; set; }

        bool CheckCanRun();

        void DoRunning();
        
        void CancelRunning();
        
        #endregion

        #region Jump

        bool Jumpable { get; set; }
        
        BindableProperty<float> JumpHeight { get; set; }
        
        BindableProperty<float> JumpSpeed { get; set; }

        bool CheckJumpable();

        void DoJump();

        void Land();

        #endregion

        #region  Crouch

        bool Crouchable { get; set; }
        
        BindableProperty<float>  CrouchSpeed { get; set; }
        
        BindableProperty<float> CrouchChangeSpeedRatio { get; set; }
        
        BindableProperty<float> CrouchChangeHeightRatio { get; set; }
        
        void DoCrouch();
        
        bool CheckCanCrouch();
        
        void CancelCrouch();
        
        bool CheckCanCancelCrouch();

        #endregion

        #region Dash

        bool Dashable { get; set; }
        
        BindableProperty<float>  DashSpeed { get; set; }
        
        BindableProperty<float> DashFrequency { get; set; }

        bool CheckCanDash();
        
        void DoDash();

        #endregion
    }
}