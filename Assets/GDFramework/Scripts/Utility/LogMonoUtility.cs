using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GDFramework_Core.Utility
{
    public class LogMonoUtility : BasicToolMonoUtility, IPointerDownHandler
    {
        private List<LogMessage> logs = new(); // 存储所有日志

        private List<LogMessage> collapsedLogs = new(); // 存储累计后的日志

        private Vector2 scrollPosition;

        private Vector2 stackScrollPosition;

        private bool showLog = true;

        private bool showWarning = true;

        private bool showError = true;

        private bool collapse = false; // 是否合并相同日志

        private bool autoScroll = true; // 自动滚动

        // 日志结构体
        private class LogMessage
        {
            public string message;
            public string stackTrace;
            public LogType type;
            public int count; // 日志的累计次数
            public string time; // 日志生成的时间
            public bool isExpanded; // 是否展开日志详情

            public LogMessage(string msg, string stack, LogType logType)
            {
                message = msg;
                stackTrace = stack;
                type = logType;
                count = 1; // 初始计数为1
                time = DateTime.Now.ToString("HH:mm:ss"); // 记F录日志生成的时间
                isExpanded = false; // 默认不展开
            }
        }

        protected override void InitUtility()
        {
            Main.Interface.RegisterUtility(this);
            Application.logMessageReceived += HandleLog;
        }

        protected override void DeInitUtility()
        {
            base.DeInitUtility();
            Application.logMessageReceived -= HandleLog;
        }


        public static void AddLog(string log)
        {
            Debug.Log(log);
        }

        public static void AddErrorLog(string log)
        {
            Debug.LogError(log);
        }


        // 处理日志
        private void HandleLog(string logString, string stackTrace, LogType type)
        {
            // 创建新日志
            var newLog = new LogMessage(logString, stackTrace, type);

            // 无论是否累计，所有日志都先按时间顺序添加到 logs 中
            logs.Add(newLog);

            // 如果启用了累计功能
            if (collapse)
            {
                var found = false;
                for (var i = 0; i < collapsedLogs.Count; i++)
                    if (collapsedLogs[i].message == logString && collapsedLogs[i].type == type)
                    {
                        collapsedLogs[i].count++; // 累计相同日志
                        collapsedLogs[i].time = newLog.time; // 更新为最新时间
                        collapsedLogs[i].stackTrace = stackTrace; // 更新堆栈信息
                        found = true;
                        break;
                    }

                if (!found) collapsedLogs.Add(newLog); // 如果没有相同的日志，添加新日志
            }
            else
            {
                collapsedLogs.Add(newLog); // 在非累计模式下直接添加日志
            }

            autoScroll = true; // 每次添加新日志后，自动滚动
        }

        protected override void DrawGUI()
        {
            // 如果日志系统不显示，直接返回
            if (!isShowing) return;

            // 包围日志系统的box
            GUILayout.BeginArea(new Rect(10, 40, 700, 600), GUI.skin.box); // 日志系统区域向下移动，避免与按钮重叠

            // 上方控制按钮
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(showLog ? "隐藏Logs" : "显示Logs")) showLog = !showLog;

            if (GUILayout.Button(showWarning ? "隐藏Warnings" : "显示Warnings")) showWarning = !showWarning;

            if (GUILayout.Button(showError ? "隐藏Errors" : "显示Errors")) showError = !showError;

            // 是否合并相同日志
            if (GUILayout.Button(collapse ? "取消累计" : "开启累计"))
            {
                collapse = !collapse;
                UpdateCollapsedLogs(); // 切换累计模式时更新日志
            }

            if (GUILayout.Button("清除日志"))
            {
                logs.Clear();
                collapsedLogs.Clear();
            }

            GUILayout.EndHorizontal();

            // 日志区域滚动条
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Height(400));

            // 根据是否启用累计模式，选择显示的日志列表
            var logList = collapse ? collapsedLogs : logs;

            foreach (var log in logList)
            {
                // 根据日志类型筛选
                if ((log.type == LogType.Log && !showLog) || (log.type == LogType.Warning && !showWarning) ||
                    (log.type == LogType.Error && !showError))
                    continue;

                // 根据日志类型设置颜色
                switch (log.type)
                {
                    case LogType.Log:
                        GUI.contentColor = Color.white;
                        break;
                    case LogType.Warning:
                        GUI.contentColor = Color.yellow;
                        break;
                    case LogType.Error:
                        GUI.contentColor = Color.red;
                        break;
                }

                // 显示日志信息及时间
                var logText = $"{log.time} ---------- {log.message}";
                if (log.count > 1) logText += $" -----(x{log.count})"; // 如果日志重复，显示重复次数

                // 点击日志信息展开/收起堆栈信息
                if (GUILayout.Button(logText)) log.isExpanded = !log.isExpanded;
            }

            GUI.contentColor = Color.white; // 重置颜色
            GUILayout.EndScrollView();

            // 自动滚动到最新日志（如果用户没有手动拖动）
            if (autoScroll) scrollPosition.y = float.MaxValue;

            GUILayout.EndArea();

            // 额外的 Box 用于显示堆栈信息，支持滚动
            foreach (var log in logList)
                if (log.isExpanded)
                {
                    // 创建一个新的 box 来显示堆栈信息
                    GUILayout.BeginArea(new Rect(10, 650, 700, 150), GUI.skin.box); // 在主 box 下面创建一个新的 box
                    GUI.contentColor = Color.gray; // 设置为灰色

                    GUILayout.Label($"堆栈信息: {log.time}");

                    stackScrollPosition = GUILayout.BeginScrollView(stackScrollPosition, GUILayout.Width(680),
                        GUILayout.Height(120));
                    GUILayout.TextArea(log.stackTrace, GUILayout.ExpandHeight(true)); // 确保堆栈信息自适应高度
                    GUILayout.EndScrollView();

                    GUILayout.EndArea();
                }
        }

        // 更新累计日志列表
        private void UpdateCollapsedLogs()
        {
            collapsedLogs.Clear();
            if (collapse)
                foreach (var log in logs)
                {
                    var found = false;
                    for (var i = 0; i < collapsedLogs.Count; i++)
                        if (collapsedLogs[i].message == log.message && collapsedLogs[i].type == log.type)
                        {
                            collapsedLogs[i].count++; // 累计相同日志
                            collapsedLogs[i].time = log.time; // 更新为最新时间
                            collapsedLogs[i].stackTrace = log.stackTrace; // 更新堆栈信息
                            found = true;
                            break;
                        }

                    if (!found)
                        collapsedLogs.Add(new LogMessage(log.message, log.stackTrace, log.type)
                        {
                            time = log.time, // 保留最新日志的时间
                            count = log.count // 保留累计次数
                        });
                }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            autoScroll = false;
        }
    }
}