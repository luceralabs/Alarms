using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LuceraLabs.Alarms;
using System.Threading.Tasks;

namespace LuceraLabs.Alarms.UnitTests
{
    [TestClass]
    public class AlarmUnitTests
    {
        private bool ScheduleHasExecuted = false;

        [TestMethod]
        public void Alarm_Schedule()
        {
            var timezone = AlarmTimeZone.GetSystemDefault();
            var alarm = new Alarm()
            {
                DaysOfWeek = new DayOfWeek[] { },
                LocalTime = timezone.CurrentLocalDateTime.AddSeconds(1).TimeOfDay
            };
            alarm.Schedule(() => { ScheduleHasExecuted = true; }, timezone).Wait();
            Task.Delay(2 * 1000);
            Assert.IsTrue(ScheduleHasExecuted);
        }
    }
}
