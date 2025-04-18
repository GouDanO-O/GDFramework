using GDFrameworkCore;

namespace GDFramework.World.Object
{
    public abstract class ObjMovement : IObjMovement
    {
        public virtual void InitMovement()
        {
            
        }
        
        public BindableProperty<float> CurMoveSpeed { get; set; }
        
        public BindableProperty<float> MoveSpeed { get; set; }
        
        public bool Runable { get; set; }
        
        public BindableProperty<float> RunningSpeed { get; set; }
        
        public bool CheckCanRun()
        {
            throw new System.NotImplementedException();
        }

        public void DoRunning()
        {
            throw new System.NotImplementedException();
        }

        public void CancelRunning()
        {
            throw new System.NotImplementedException();
        }

        public bool Jumpable { get; set; }
        
        public BindableProperty<float> JumpHeight { get; set; }
        
        public BindableProperty<float> JumpSpeed { get; set; }
        
        public bool CheckJumpable()
        {
            throw new System.NotImplementedException();
        }

        public void DoJump()
        {
            throw new System.NotImplementedException();
        }

        public void Land()
        {
            throw new System.NotImplementedException();
        }

        public bool Crouchable { get; set; }
        
        public BindableProperty<float> CrouchSpeed { get; set; }
        
        public BindableProperty<float> CrouchChangeSpeedRatio { get; set; }
        
        public BindableProperty<float> CrouchChangeHeightRatio { get; set; }
        
        public void DoCrouch()
        {
            throw new System.NotImplementedException();
        }

        public bool CheckCanCrouch()
        {
            throw new System.NotImplementedException();
        }

        public void CancelCrouch()
        {
            throw new System.NotImplementedException();
        }

        public bool CheckCanCancelCrouch()
        {
            throw new System.NotImplementedException();
        }

        public bool Dashable { get; set; }
        
        public BindableProperty<float> DashSpeed { get; set; }
        
        public BindableProperty<float> DashFrequency { get; set; }
        
        public bool CheckCanDash()
        {
            throw new System.NotImplementedException();
        }

        public void DoDash()
        {
            throw new System.NotImplementedException();
        }
    }
}