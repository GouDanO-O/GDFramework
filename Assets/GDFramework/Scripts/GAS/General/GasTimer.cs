using System;
using UnityEngine;

namespace GDFramework_Core.GAS.General
{
    public class GasTimer
    {
        /// <summary>
        /// 矫正时间差(服务器客户端时间差/暂停游戏导致的时间差)
        /// </summary>
        private static int _deltaTime;

        /// <summary>
        /// 当前时间+时间差
        /// </summary>
        /// <returns></returns>
        public static long Timestamp()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + _deltaTime;
        }

        public static long TimestampSeconds()
        {
            return Timestamp() / 1000;
        }

        private static int _currentFrameCount;

        public static int CurrentFrameCount => _currentFrameCount;

        private static long _startTimestamp;

        public static long StartTimestamp => _startTimestamp;

        private static long _pauseTimestamp;

        private static int _frameRate = 60;

        public static int FrameRate => _frameRate;

        /// <summary>
        /// 设置起始的时间差(减少因时间导致的误差)
        /// </summary>
        public static void InitStartTimestamp()
        {
            _startTimestamp = Timestamp();
        }

        /// <summary>
        /// 更新当前已运行的帧数
        /// </summary>
        public static void UpdateCurrentFrameCount()
        {
            _currentFrameCount = Mathf.FloorToInt((Timestamp() - _startTimestamp) / 1000f * FrameRate);
        }

        /// <summary>
        /// 暂停
        /// </summary>
        public static void Pause()
        {
            _pauseTimestamp = Timestamp();
        }

        /// <summary>
        /// 取消暂停
        /// 更新当前时间差
        /// </summary>
        public static void Unpause()
        {
            _deltaTime -= (int)(Timestamp() - _pauseTimestamp);
        }
    }
}