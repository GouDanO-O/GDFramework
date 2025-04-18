using System.Collections.Generic;
using GDFramework.World.Map;
using GDFramework.World.Times;
using GDFrameworkCore;
using Sirenix.OdinInspector;
using Unity.VisualScripting;

namespace GDFramework.World.Weather
{
    /// <summary>
    /// 天气系统类似塞尔达旷野之息的天气系统
    /// 天气只会随着时间的改变而改变
    /// 不同地区的天气也会不一样
    /// 根据不同的地图区块去更新天气
    /// </summary>
    public class WorldWeatherManager : AbstractSystem,IUnRegisterList
    {
        private WorldWeatherModel _worldWeatherModel;
        
        public List<IUnRegister> UnregisterList { get; }
        
        protected override void OnInit()
        {
            UnregisterList.Add(this.RegisterEvent<SWorldTimeHourChangeEvent>((data) =>
            {
                WorldTimeHourChange(data.CurrentTime);
            }));
            
            UnregisterList.Add(this.RegisterEvent<SWorldTimeDayChangeEvent>((data) =>
            {
                WorldTimeDayChange(data.CurrentTime);
            }));
            
            UnregisterList.Add(this.RegisterEvent<SWorldTimeMouthChangeEvent>((data) =>
            {
                WorldTimeMouthChange(data.CurrentTime);
            }));
            
            UnregisterList.Add(this.RegisterEvent<SUpdateEnteredWorldMapAreaEvent>((data) =>
            {
                WorldMapAreaChange();
            }));
        }

        public void InitWeatherManager()
        {
            
        }

        public void DeInitWeatherManager()
        {
           
        }

        public void FixedUpdateWeather()
        {
            
        }
        
        public void UpdateWeather()
        {
            
        }

        /// <summary>
        /// 改变小时
        /// </summary>
        /// <param name="dateTime"></param>
        private void WorldTimeHourChange(GameDateTime dateTime)
        {
            _worldWeatherModel.CaculateNextWeather();
        }
        
        /// <summary>
        /// 改变日
        /// </summary>
        /// <param name="dateTime"></param>
        private void WorldTimeDayChange(GameDateTime dateTime)
        {
            
        }
        
        /// <summary>
        /// 月份改变
        /// </summary>
        /// <param name="dateTime"></param>
        private void WorldTimeMouthChange(GameDateTime dateTime)
        {
            
        }

        /// <summary>
        /// 玩家所属地块变化
        /// </summary>
        private void WorldMapAreaChange()
        {
            
        }
    }
}