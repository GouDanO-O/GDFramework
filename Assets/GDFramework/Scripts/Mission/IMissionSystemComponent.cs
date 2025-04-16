namespace GDFramework.Mission
{
    public interface IMissionSystemComponent<T>
    {
        /// <summary>
        /// 任务启动时触发该函数
        /// </summary>
        /// <param name="mission"></param>
        public void OnMissionStarted(Mission<T> mission);

        /// <summary>
        /// 任务被移除时触发该函数
        /// </summary>
        /// <param name="mission"></param>
        /// <param name="isFinished">任务是否已经完成</param>
        public void OnMissionRemoved(Mission<T> mission, bool isFinished);

        /// <summary>
        /// 任务状态变化时触发该函数
        /// </summary>
        /// <param name="mission"></param>
        /// <param name="isFinished"></param>
        public void OnMissionStatusChanged(Mission<T> mission, bool isFinished);
    }
}