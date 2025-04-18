using System;
using System.Collections.Generic;
using GDFrameworkCore;
using GDFrameworkExtend.UIKit;

namespace GDFramework.World.Times
{
    public class WorldTimeModel : AbstractModel
    {
        /// <summary>
        /// 日出
        /// </summary>
        protected float SunRiseTime;

        /// <summary>
        /// 日落
        /// </summary>
        protected float SunSetTime;


        /// <summary>
        /// 当前时间日期
        /// </summary>
        public BindableProperty<GameDateTime> CurDateTime;

        protected override void OnInit()
        {

        }

        /// <summary>
        /// 重设时间
        /// </summary>
        private void InitTime()
        {
            CurDateTime = new BindableProperty<GameDateTime>(new GameDateTime(1, 1, 1, 7, 0, 0));
        }

        /* ---------- 秒 ---------- */
        public void AddSecond() => AddSeconds(1);

        public void AddSeconds(int sec)
        {
            CurDateTime.Value.AddSeconds(sec);
        }

        /* ---------- 分 ---------- */
        public void AddMinute() => AddMinutes(1);

        public void AddMinutes(int min)
        {
            CurDateTime.Value.AddMinutes(min);
        }

        /* ---------- 时 ---------- */
        public void AddHour() => AddHours(1);

        public void AddHours(int hrs)
        {
            CurDateTime.Value.AddHours(hrs);
            this.SendEvent<SWorldTimeHourChangeEvent>(new SWorldTimeHourChangeEvent(CurDateTime.Value));
        }

        /* ---------- 日 ---------- */
        public void AddDay() => AddDays(1);

        public void AddDays(int days)
        {
            CurDateTime.Value.AddDays(days);
            this.SendEvent<SWorldTimeDayChangeEvent>(new SWorldTimeDayChangeEvent(CurDateTime.Value));
        }

        /* ---------- 月 ---------- */
        public void AddMonth() => AddMonths(1);

        public void AddMonths(int months)
        {
            CurDateTime.Value.AddMonths(months);
            this.SendEvent<SWorldTimeMouthChangeEvent>(new SWorldTimeMouthChangeEvent(CurDateTime.Value));
        }

        /* ---------- 年 ---------- */
        public void AddYear() => AddYears(1);

        public void AddYears(int yrs)
        {
            CurDateTime.Value.AddYears(yrs);
        }
        
        public void SaveTime()
        {
            
        }
    }

    [Serializable]
    public class GameDateTime
    {
        private const int OneMinuteSeconds = 60;
        
        private const int OneHourMinutes = 60;
        
        private const int OneDayHours = 24;
        
        private int OneYearMonths => MonthlyDayCountList.Count;

        /// <summary>每个月的天数</summary>
        private readonly List<int> MonthlyDayCountList = new()
        {
            30, 28, 30, 31, 30, 30,
            30, 30, 31, 30, 30, 30
        };

        /* ========= 日期时间字段 ========= */
        public int Second { get;private set; }
        
        public int Minute { get;private set; }
        
        public int Hour { get;private set; }
        
        public int Day { get;private set; }
        
        public int Month { get;private set; }
        
        public int Year { get;private set; }

        public GameDateTime(int year, int month, int day, int hour, int minute, int second)
        {
            Year = year;
            Month = month;
            Day = day;
            Hour = hour;
            Minute = minute;
            Second = second;
        }

        /* ---------- 秒 ---------- */
        public void AddSecond() => AddSeconds(1);

        public void AddSeconds(int sec)
        {
            Second += sec;
            while (Second >= OneMinuteSeconds)
            {
                Second -= OneMinuteSeconds;
                AddMinute();
            }
        }

        /* ---------- 分 ---------- */
        public void AddMinute() => AddMinutes(1);

        public void AddMinutes(int min)
        {
            Minute += min;
            while (Minute >= OneHourMinutes)
            {
                Minute -= OneHourMinutes;
                AddHour();
            }
        }

        /* ---------- 时 ---------- */
        public void AddHour() => AddHours(1);

        public void AddHours(int hrs)
        {
            Hour += hrs;
            while (Hour >= OneDayHours)
            {
                Hour -= OneDayHours;
                AddDay();
            }
        }

        /* ---------- 日 ---------- */
        public void AddDay() => AddDays(1);

        public void AddDays(int days)
        {
            Day += days;
            while (true)
            {
                int daysInCurMonth = MonthlyDayCountList[Month - 1];
                if (Day <= daysInCurMonth) break;

                Day -= daysInCurMonth;
                AddMonth();
            }
        }

        /* ---------- 月 ---------- */
        public void AddMonth() => AddMonths(1);

        public void AddMonths(int months)
        {
            Month += months;
            while (Month > OneYearMonths)
            {
                Month -= OneYearMonths;
                AddYear();
            }
        }

        /* ---------- 年 ---------- */
        public void AddYear() => AddYears(1);

        public void AddYears(int yrs)
        {
            Year += yrs;
        }

        public override string ToString() =>
            $"{Year:D4}-{Month:D2}-{Day:D2} {Hour:D2}:{Minute:D2}:{Second:D2}";
    }
}