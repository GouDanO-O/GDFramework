﻿using System;
using System.Text;
using MoonSharp.Interpreter.Diagnostics.PerformanceCounters;

namespace MoonSharp.Interpreter.Diagnostics
{
    /// <summary>
    /// A single object of this type exists for every script and gives access to performance statistics.
    /// </summary>
    public class PerformanceStatistics
    {
        private IPerformanceStopwatch[] m_Stopwatches = new IPerformanceStopwatch[(int)PerformanceCounter.LastValue];

        private static IPerformanceStopwatch[] m_GlobalStopwatches =
            new IPerformanceStopwatch[(int)PerformanceCounter.LastValue];

        private bool m_Enabled = false;


        /// <summary>
        /// Gets or sets a value indicating whether this collection of performance stats is enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if enabled; otherwise, <c>false</c>.
        /// </value>
        public bool Enabled
        {
            get => m_Enabled;
            set
            {
                if (value && !m_Enabled)
                {
                    if (m_GlobalStopwatches[(int)PerformanceCounter.AdaptersCompilation] == null)
                        m_GlobalStopwatches[(int)PerformanceCounter.AdaptersCompilation] =
                            new GlobalPerformanceStopwatch(PerformanceCounter.AdaptersCompilation);

                    for (var i = 0; i < (int)PerformanceCounter.LastValue; i++)
                        m_Stopwatches[i] = m_GlobalStopwatches[i] ?? new PerformanceStopwatch((PerformanceCounter)i);
                }
                else if (!value && m_Enabled)
                {
                    m_Stopwatches = new IPerformanceStopwatch[(int)PerformanceCounter.LastValue];
                    m_GlobalStopwatches = new IPerformanceStopwatch[(int)PerformanceCounter.LastValue];
                }

                m_Enabled = value;
            }
        }


        /// <summary>
        /// Gets the result of the specified performance counter .
        /// </summary>
        /// <param name="pc">The PerformanceCounter.</param>
        /// <returns></returns>
        public PerformanceResult GetPerformanceCounterResult(PerformanceCounter pc)
        {
            var pco = m_Stopwatches[(int)pc];
            return pco != null ? pco.GetResult() : null;
        }

        /// <summary>
        /// Starts a stopwatch.
        /// </summary>
        /// <returns></returns>
        internal IDisposable StartStopwatch(PerformanceCounter pc)
        {
            var pco = m_Stopwatches[(int)pc];
            return pco != null ? pco.Start() : null;
        }

        /// <summary>
        /// Starts a stopwatch.
        /// </summary>
        /// <returns></returns>
        internal static IDisposable StartGlobalStopwatch(PerformanceCounter pc)
        {
            var pco = m_GlobalStopwatches[(int)pc];
            return pco != null ? pco.Start() : null;
        }

        /// <summary>
        /// Gets a string with a complete performance log.
        /// </summary>
        /// <returns></returns>
        public string GetPerformanceLog()
        {
            var sb = new StringBuilder();

            for (var i = 0; i < (int)PerformanceCounter.LastValue; i++)
            {
                var res = GetPerformanceCounterResult((PerformanceCounter)i);
                if (res != null)
                    sb.AppendLine(res.ToString());
            }

            return sb.ToString();
        }
    }
}