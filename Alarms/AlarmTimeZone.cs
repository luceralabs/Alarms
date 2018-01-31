using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using NodaTime;
using NodaTime.TimeZones;

namespace LuceraLabs.Alarms
{
    public class AlarmTimeZone
    {
        /// <summary>
        /// The NodaTime timezone
        /// </summary>
        protected DateTimeZone _TimeZone;

        /// <summary>
        /// The tz database identifier for the IANA Time Zone Database
        /// <para>See <see cref="https://www.iana.org/time-zones"/> for more details</para>
        /// </summary>
        public string ID
        {
            get { return _TimeZone is null ? null : _TimeZone.ToString(); }
            set { _TimeZone = FindTimezone(value); }
        }

        /// <summary>
        /// Returns the NodaTimeZone for the provided identifier
        /// </summary>
        /// <param name="timezone"></param>
        /// <returns>The NodaTimeZone with the ID associated with the input</returns>
        protected static DateTimeZone FindTimezone(string timezone)
        {
            if (timezone is null || timezone.Equals(""))
                return null;

            var tzdbTimeZone = from t in NodaTime.TimeZones.TzdbDateTimeZoneSource.Default.GetIds()
                               where t.ToLower().Equals(timezone.ToLower())
                               select t;
            if (!tzdbTimeZone.Any())
                throw new InvalidTimeZoneException($"'{timezone}' is not a valid time zone identifier");

            return TzdbDateTimeZoneSource.Default.ForId(tzdbTimeZone.First());
        }

        /// <summary>
        /// Creates a new instance of WakeTimeZone
        /// </summary>
        public AlarmTimeZone() { }

        /// <summary>
        /// Creates a new instance of WakeTimeZone using the supplied timezone identifier
        /// </summary>
        /// <param name="timezone">Timezone identifier</param>
        public AlarmTimeZone(string timezone)
        {
            _TimeZone = FindTimezone(timezone);
        }

        /// <summary>
        /// The current offset of the timezone from UTC
        /// </summary>
        public TimeSpan UtcOffset { get { return _TimeZone.GetUtcOffset(Instant.FromDateTimeUtc(DateTime.UtcNow)).ToTimeSpan(); } }

        /// <summary>
        /// The current local date and time in the timzone
        /// </summary>
        public DateTime CurrentLocalDateTime { get { return DateTime.SpecifyKind(DateTime.UtcNow.Add(UtcOffset), DateTimeKind.Unspecified); } }

        /// <summary>
        /// Returns the UTC time where the local time in the timezone will be the provided value
        /// </summary>
        /// <param name="localtime">The time of day in the timezone</param>
        /// <returns>The UTC time that is equivalent to the local time in the timezone</returns>
        public DateTime NextInstance(TimeSpan localtime)
        {
            return CurrentLocalDateTime.TimeOfDay < localtime ? DateTime.UtcNow.Add(localtime - CurrentLocalDateTime.TimeOfDay) : DateTime.UtcNow.Add(localtime.Add(new TimeSpan(1, 0, 0, 0)) - CurrentLocalDateTime.TimeOfDay);
        }

        /// <summary>
        /// Returns the next instance of the provided local day of week and time in the timezone
        /// </summary>
        /// <param name="localtime">The time of day in the timezone</param>
        /// <param name="dayOfWeek">The day of week in the timezone</param>
        /// <returns>The next UTC instance that is equivalent to the next instance of the provided day of week and time in the timzone</returns>
        public DateTime NextInstance(TimeSpan localtime, DayOfWeek dayOfWeek)
        {
            int dayOffset = 0;
            while (NextInstance(localtime).Add(new TimeSpan(dayOffset, 0, 0, 0)).DayOfWeek != dayOfWeek)
                dayOffset++;
            return NextInstance(localtime).Add(new TimeSpan(dayOffset, 0, 0, 0));
        }

        /// <summary>
        /// Returns the next instance of the provided alarm in the timezone
        /// </summary>
        /// <param name="alarm"></param>
        /// <returns></returns>
        public DateTime NextInstance(IAlarm alarm)
        {
            if (alarm.DaysOfWeek.Length == 0)
                return NextInstance(alarm.LocalTime);
            List<DateTime> nextInstances = new List<DateTime>();
            foreach (DayOfWeek day in alarm.DaysOfWeek.Distinct())
                nextInstances.Add(NextInstance(alarm.LocalTime, day));
            return nextInstances.OrderBy(d => d).First();
        }

        /// <summary>
        /// Returns the UTC time where the local time in the timezone will be the provided value
        /// </summary>
        /// <param name="localtime">The time of day in the timezone</param>
        /// <returns>The UTC time that is equivalent to the local time in the timezone</returns>
        public DateTime LastInstance(TimeSpan localtime)
        {
            return CurrentLocalDateTime.TimeOfDay >= localtime ? DateTime.UtcNow.Subtract(CurrentLocalDateTime.TimeOfDay - localtime) : DateTime.UtcNow.Subtract(new TimeSpan(1, 0, 0, 0)).Subtract(CurrentLocalDateTime.TimeOfDay - localtime);
        }

        /// <summary>
        /// Returns the next instance of the provided local day of week and time in the timezone
        /// </summary>
        /// <param name="localtime">The time of day in the timezone</param>
        /// <param name="dayOfWeek">The day of week in the timezone</param>
        /// <returns>The last UTC instance that is equivalent to the next instance of the provided day of week and time in the timzone</returns>
        public DateTime LastInstance(TimeSpan localtime, DayOfWeek dayOfWeek)
        {
            int dayOffset = 0;
            while (LastInstance(localtime).Subtract(new TimeSpan(dayOffset, 0, 0, 0)).DayOfWeek != dayOfWeek)
                dayOffset++;
            return LastInstance(localtime).Subtract(new TimeSpan(dayOffset, 0, 0, 0));
        }

        public DateTime LastInstance(IAlarm alarm)
        {
            if (alarm.DaysOfWeek.Length == 0)
                return LastInstance(alarm.LocalTime);
            List<DateTime> lastInstances = new List<DateTime>();
            foreach (DayOfWeek day in alarm.DaysOfWeek.Distinct())
                lastInstances.Add(LastInstance(alarm.LocalTime));
            return lastInstances.OrderBy(d => d).FirstOrDefault();
        }

        /// <summary>
        /// Verifies that the supplied string is a valid timezone identifier
        /// </summary>
        /// <param name="timezone">The timezone identifier to check</param>
        /// <returns>True if the supplied string is a valid timezone identifier, false otherwise</returns>
        public static bool IsValidTimezone(string timezone)
        {
            try
            {
                new AlarmTimeZone(timezone);
            }
            catch (InvalidTimeZoneException)
            {
                return false;
            }
            return true;
        }

        public static AlarmTimeZone GetSystemDefault()
        {
            return new AlarmTimeZone(DateTimeZoneProviders.Tzdb.GetSystemDefault().Id);
        }
    }
}
