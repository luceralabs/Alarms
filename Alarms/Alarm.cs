using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

namespace LuceraLabs.Alarms
{
    public class Alarm : IAlarm
    {
        /// <summary>
        /// The time of the alarm
        /// </summary>
        public TimeSpan LocalTime { set; get; }

        /// <summary>
        /// The days of the week the alarm will go off
        /// <para>0 is Sunday, 1 is Monday, ..., 6 is Saturday</para>
        /// <para>An empty array indicates a one-time alarm</para>
        /// </summary>
        protected DayOfWeek[] _DaysOfWeek;
        public DayOfWeek[] DaysOfWeek
        {
            get
            {
                List<DayOfWeek> daylist = new List<DayOfWeek>();
                return _DaysOfWeek is null ? new DayOfWeek[] { } : _DaysOfWeek;
            }
            set
            {
                List<DayOfWeek> dayList = new List<DayOfWeek>();
                if(value.Length == 0)
                {
                    _DaysOfWeek = new DayOfWeek[] { };
                    return;
                }
                foreach(DayOfWeek day in value)
                {
                    if (!dayList.Contains(day))
                        dayList.Add(day);
                }
                _DaysOfWeek = dayList.ToArray();
            }
        }

        /// <summary>
        /// A list of custom properties of the alarm
        /// </summary>
        public Dictionary<string, object> Properties { set; get; } = new Dictionary<string, object>();

        /// <summary>
        /// Returns true if the alarm is a one-time alarm
        /// </summary>
        public bool IsOneTime
        {
            get { return _DaysOfWeek is null || _DaysOfWeek.Length == 0 ? true : false; }
        }

        /// <summary>
        /// The next instance of the alarm in the provided timezone
        /// </summary>
        /// <param name="timezone"></param>
        /// <returns>The next instance of the alarm in the provided timezone</returns>
        public DateTime NextInstance(AlarmTimeZone timezone)
        {
            return IsOneTime ? timezone.NextInstance(LocalTime) : timezone.NextInstance(this);
        }

        /// <summary>
        /// The last instance of the alarm in the provided timezone
        /// </summary>
        /// <param name="timezone"></param>
        /// <returns>The last instance of the alarm in the provided timezone</returns>
        public DateTime LastInstance(AlarmTimeZone timezone)
        {
            return IsOneTime ? timezone.LastInstance(LocalTime) : timezone.LastInstance(this);
        }

        /// <summary>
        /// Schedules an action to occur at the next alarm
        /// </summary>
        /// <param name="action">The action to occur</param>
        /// <param name="timezone">The timezone in which the alarm will take place</param>
        /// <returns></returns>
        public async Task ScheduleAsync(Action action, AlarmTimeZone timezone)
        {
            await Task.Delay(NextInstance(timezone) - DateTime.UtcNow).ContinueWith((result) => action.Invoke());
        }
    }
}
