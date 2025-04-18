namespace GDFramework.World.Weather
{
    /// <summary>
    /// 改变天气
    /// </summary>
    public struct SWorldWeatherChangeEvent
    {
        public EWorldWeatherType willChangeWeather;

        public SWorldWeatherChangeEvent(EWorldWeatherType willChangeWeather)
        {
            this.willChangeWeather = willChangeWeather;
        }
    }
}