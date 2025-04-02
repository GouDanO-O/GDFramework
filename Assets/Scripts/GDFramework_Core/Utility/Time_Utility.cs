using UnityEngine;

namespace GDFramework_Core.Utility
{
    public class Time_Utility : BasicTool_Utility
    {
        public override void InitUtility()
        {
            Main.Interface.RegisterUtility(this);
        }
        
        
        /// <summary>
        /// 将秒转换成时分秒
        /// </summary>
        /// <param name="totalSeconds"></param>
        /// <returns></returns>
        public string ConvertToTimeFormat(float totalSeconds=0)
        {
            int hours = Mathf.FloorToInt(totalSeconds / 3600);
            int minutes = Mathf.FloorToInt((totalSeconds % 3600) / 60);
            int seconds = Mathf.FloorToInt(totalSeconds % 60);

            return string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
        }
    } 
}

