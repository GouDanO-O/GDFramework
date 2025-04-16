namespace GDFramework.Mission
{
    public abstract class MissionRequirementHandle<T>
    {
        private readonly MissionRequirement<T> _require;
        
        protected MissionRequirementHandle(MissionRequirement<T> require)
        {
            _require = require;
        }
        
        /// <summary>
        /// 发送一条消息给玩家
        /// </summary>
        /// <param name="message"></param>
        /// <param name="hasStatusChanged"></param>
        /// <returns></returns>
        public bool SendMessage(T message, out bool hasStatusChanged)
        {
            hasStatusChanged = false;
            if (!_require.CheckMessage(message)) 
                return false;
            hasStatusChanged = true;
            return UseMessage(message);
        }

        /// <summary>
        /// 应用某条消息并返回当前需求是否已经完成
        /// </summary>
        /// <param name="message">目标消息</param>
        /// <returns></returns>
        protected abstract bool UseMessage(T message);
    }
}