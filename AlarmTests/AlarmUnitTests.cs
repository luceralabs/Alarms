using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LuceraLabs.Alarms;
using System.Threading.Tasks;

namespace LuceraLabs.Alarms.UnitTests
{
    [TestClass]
    public class AlarmUnitTests
    {
        private AlarmTimeZone TimeZone { get { return AlarmTimeZone.GetSystemDefault(); } }
        private bool ScheduleActionHasExecuted = false;

        [TestMethod]
        public void Alarm_Schedule_Action()
        {
            var alarm = new Alarm()
            {
                DaysOfWeek = new DayOfWeek[] { },
                LocalTime = TimeZone.CurrentLocalDateTime.AddSeconds(1).TimeOfDay
            };
            Assert.IsFalse(ScheduleActionHasExecuted);
            alarm.ScheduleAsync(() => { ScheduleActionHasExecuted = true; }, TimeZone).Wait();
            Assert.IsTrue(ScheduleActionHasExecuted);
        }

        [TestMethod]
        public void Alarm_IsOneTime()
        {
            var alarm = new Alarm()
            {
                DaysOfWeek = new DayOfWeek[] { }
            };
            Assert.IsTrue(alarm.IsOneTime);

            alarm.DaysOfWeek = new DayOfWeek[] { DayOfWeek.Sunday };
            Assert.IsFalse(alarm.IsOneTime);
        }
    }
}
