using System.Collections;
using System.Collections.Generic;
using GDFrameworkCore;

namespace GDFramework.World.Weather
{
    public enum EWorldWeatherType
    {
        Sunny,
        Rainy,
        Rainstorm,
        Cloudy,
        Windy,
        Hurricane,
        Snowy,
        Snowstorm,
    }
    
    public class WorldMapWeatherData : ICanSendEvent
    {
        public EWorldWeatherType _curWorldWeatherType;
        
        /// <summary>
        /// 接下来xx小时内的天气变化
        /// </summary>
        private Queue<EWorldWeatherType> _laterWeatherType;
        
        public IArchitecture GetArchitecture()
        {
            return Main.Interface;
        }

        /// <summary>
        /// 加载存档中当前地图块的天气
        /// </summary>
        public void LoadSavingLaterWeathers()
        {
            
        }

        /// <summary>
        /// 获取下一个小时的天气
        /// </summary>
        public void CaculateNextWeather()
        {
            ChangeWeatherType(_laterWeatherType.Dequeue());
            AddNextWeatherType();
        }

        /// <summary>
        /// 天气出对一个
        /// 就要对应的入队一个
        /// </summary>
        private void AddNextWeatherType()
        {
            
        }

        /// <summary>
        /// 根据当前进入的区块来更新当前区块的天气
        /// </summary>
        private void UpdateWeatherArea()
        {
            
        }
        
        /// <summary>
        /// 改变天气
        /// </summary>
        /// <param name="weatherType"></param>
        public void ChangeWeatherType(EWorldWeatherType weatherType)
        {
            if (_curWorldWeatherType != weatherType)
            {
                _curWorldWeatherType = weatherType;
                this.SendEvent<SWorldWeatherChangeEvent>(new SWorldWeatherChangeEvent(weatherType));
            }
        }
    }
}