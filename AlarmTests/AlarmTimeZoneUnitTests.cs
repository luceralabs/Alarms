using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LuceraLabs.Alarms;

namespace LuceraLabs.Alarms.UnitTests
{
    [TestClass]
    public class AlarmTimeZoneUnitTests
    {
        private TimeZoneInfo WindowsTimeZone { get { return TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time"); } }
        private DateTime LocalTime { get { return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, WindowsTimeZone); } }
        private AlarmTimeZone TimeZone = new AlarmTimeZone("America/Los_Angeles");

        [TestMethod]
        public void AlarmTimeZone_UTC_Offset()
        {
            // Verify the UTC offsets are the same
            Assert.AreEqual(TimeZone.UtcOffset, WindowsTimeZone.BaseUtcOffset);
        }

        [TestMethod]
        public void AlarmTimeZone_Current_Time()
        {
            // Verify the current time is the same
            Assert.IsTrue(Math.Abs((TimeZone.CurrentLocalDateTime - LocalTime).TotalSeconds) < 1);
        }

        [TestMethod]
        public void AlarmTimeZone_Next_Instance()
        {
            // Verify a time in the future is the same

            var nextTime = LocalTime.Add(new TimeSpan(6, 0, 0));
            var nextInstanceTime = TimeZone.NextInstance(nextTime.TimeOfDay);
            var windowsNextTime = TimeZoneInfo.ConvertTimeToUtc(nextTime, WindowsTimeZone);
            Assert.IsTrue(Math.Abs((nextInstanceTime - windowsNextTime).TotalSeconds) < 1);

            // Verify a time farther in the future is the same (check for time wrap)
            nextTime = LocalTime.Add(new TimeSpan(18, 0, 0));
            nextInstanceTime = TimeZone.NextInstance(nextTime.TimeOfDay);
            windowsNextTime = TimeZoneInfo.ConvertTimeToUtc(nextTime, WindowsTimeZone);
            Assert.IsTrue(Math.Abs((nextInstanceTime - windowsNextTime).TotalSeconds) < 1);

            // Verify a particular day instance
            nextTime = LocalTime.Add(new TimeSpan(4, 6, 0, 0));
            windowsNextTime = TimeZoneInfo.ConvertTimeToUtc(nextTime, WindowsTimeZone);
            nextInstanceTime = TimeZone.NextInstance(nextTime.TimeOfDay, windowsNextTime.DayOfWeek);
            Assert.IsTrue(Math.Abs((nextInstanceTime - windowsNextTime).TotalSeconds) < 1);
        }

        [TestMethod]
        public void ALarmTimeZone_Last_Instance()
        {
            // Verify a time in the past is the same
            var lastTime = LocalTime.Subtract(new TimeSpan(6, 0, 0));
            var lastInstanceTime = TimeZone.LastInstance(lastTime.TimeOfDay);
            var windowsLastTime = TimeZoneInfo.ConvertTimeToUtc(lastTime, WindowsTimeZone);
            Assert.IsTrue(Math.Abs((lastInstanceTime - windowsLastTime).TotalSeconds) < 1);

            // Verify a time farther in the past is the same
            lastTime = LocalTime.Subtract(new TimeSpan(18, 0, 0));
            lastInstanceTime = TimeZone.LastInstance(lastTime.TimeOfDay);
            windowsLastTime = TimeZoneInfo.ConvertTimeToUtc(lastTime, WindowsTimeZone);
            Assert.IsTrue(Math.Abs((lastInstanceTime - windowsLastTime).TotalSeconds) < 1);

            // Verify a particular day instance
            lastTime = LocalTime.Subtract(new TimeSpan(4, 6, 0, 0));
            windowsLastTime = TimeZoneInfo.ConvertTimeToUtc(lastTime, WindowsTimeZone);
            lastInstanceTime = TimeZone.LastInstance(lastTime.TimeOfDay, windowsLastTime.DayOfWeek);
            Assert.IsTrue(Math.Abs((lastInstanceTime - windowsLastTime).TotalSeconds) < 1);
        }

        [TestMethod]
        public void AlarmTimeZone_Valid_TimeZone()
        {
            // Verify that a valid timezone is allowed
            Assert.IsTrue(AlarmTimeZone.IsValidTimezone("America/Los_Angeles"));

            // Verify that a invalid timezone is rejected
            Assert.IsFalse(AlarmTimeZone.IsValidTimezone("Invalid Timezone"));
        }
    }
}
