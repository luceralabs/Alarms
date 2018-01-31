using System;
using System.Collections.Generic;
using System.Text;

namespace LuceraLabs.Alarms
{
    public interface IAlarm
    {
        /// <summary>
        /// The time of the alarm
        /// </summary>
        TimeSpan LocalTime { set; get; }

        /// <summary>
        /// The days of week the alarm should repeat on. An empty array indicates the alarm shoudl occur one time only. 
        /// </summary>
        DayOfWeek[] DaysOfWeek { set; get; }
    }
}
