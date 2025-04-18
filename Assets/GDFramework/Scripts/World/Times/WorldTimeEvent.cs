namespace GDFramework.World.Times
{
    public struct SWorldTimeHourChangeEvent
    {
        public GameDateTime CurrentTime;

        public SWorldTimeHourChangeEvent(GameDateTime currentTime)
        {
            this.CurrentTime = currentTime;
        }
    }

    public struct SWorldTimeDayChangeEvent
    {
        public GameDateTime CurrentTime;
        
        public SWorldTimeDayChangeEvent(GameDateTime currentTime)
        {
            this.CurrentTime = currentTime;
        }
    }
    
    public struct SWorldTimeMouthChangeEvent
    {
        public GameDateTime CurrentTime;
        
        public SWorldTimeMouthChangeEvent(GameDateTime currentTime)
        {
            this.CurrentTime = currentTime;
        }
    }
}