using QFramework;

namespace GDFramework_Core.Models
{
    public class GameData_Model : AbstractModel
    {
        public bool isPausing { get;protected set; }
        
        public bool isCheatMode { get;protected set; }

        /// <summary>
        /// 改变暂停状态
        /// </summary>
        /// <param name="willPaused"></param>
        public void ChangGamePasuing(bool willPaused)
        {
            this.isPausing = willPaused;
        }

        /// <summary>
        /// 改变作弊模式
        /// </summary>
        /// <param name="isCheatMode"></param>
        public void ChangeCheatMode(bool isCheatMode)
        {
            this.isCheatMode = isCheatMode;
        }
        
        protected override void OnInit()
        {
            isPausing = false;
        }
    }
}