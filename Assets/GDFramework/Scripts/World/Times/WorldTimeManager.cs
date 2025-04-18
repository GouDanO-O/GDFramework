using GDFrameworkCore;

namespace GDFramework.World.Times
{
    /// <summary>
    /// 世界时间管理器
    /// </summary>
    public class WorldTimeManager : AbstractSystem
    {
        private WorldTimeModel _worldTimeModel;

        /// <summary>
        /// xx帧 == 1秒
        /// </summary>
        public int DeltaTimeAddSecondCount = 1;
        
        private int _currentDeltaTimeAddSecondCount = 0;
        
        protected override void OnInit()
        {
            
        }

        private void InitTimeManager()
        {
            _worldTimeModel = this.GetModel<WorldTimeModel>();
        }

        /// <summary>
        /// 更新世界时间
        /// </summary>
        public void FixedUpdateWorldTime()
        {
            _currentDeltaTimeAddSecondCount++;
            if (_currentDeltaTimeAddSecondCount >= DeltaTimeAddSecondCount)
            {
                _currentDeltaTimeAddSecondCount = 0;
                _worldTimeModel.AddSecond();
            }
        }
        
        /// <summary>
        /// 更新世界时间
        /// </summary>
        public void UpdateWorldTime()
        {
           
        }
    }
}