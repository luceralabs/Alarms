# Alarms
A C# library for managing alarms

This library contains two classes (AlarmTimeZone and Alarm) and an interface (IAlarm) for managing alarms. 

A unit test project is also provided. Before submitting any pull requests, please write the appropriate unit tests. 

## AlarmTimeZone
This class manages the timezone of an alarm. 

### Properties
`ID` - `string` - The IANA timezone identifier. See https://www.iana.org/time-zones and https://en.wikipedia.org/wiki/List_of_tz_database_time_zones for a list of timezones. 

`UtcOffset` - `TimeSpan` - The UTC offset of the timezone. 

`CurrentLocalDateTime` - `DateTime` - The current date and time in the timezone. 

### Methods
`NextInstance` - `DateTime` - Returns the UTC time of the next occurance.  Overloads include: 
* Local time - returns the next instance of the local time in the timezone
* Local time and day of week - returns the next instance of the local time on the provided day of week in the timezone 
* `IAlarm` - returns the next instance of the provided alarm

`LastInstance` - `DateTime` - Returns the UTC time of the last occurance.  Overloads include: 
* Local time - returns the last instance of the local time in the timezone
* Local time and day of week - returns the last instance of the local time on the provided day of week in the timezone 
* `IAlarm` - returns the last instance of the provided alarm

### Static Methods
`IsValidTimezone` - `Boolean` - Returns true if the provided string is a valid timezone identifer and false if the string is not a valid timezone identifier

`GetSystemDefault` - `AlarmTimeZone` - Returns an AlarmTimeZone instance for the system's timezone. 

## IAlarm
This interface provides a signature for an alarm

### Properties
`LocalTime` - `TimeSpan` - The local time of the alarm. 

`DaysOfWeek` - `Array` of DayOfWeek enumerated values - The days of the week that the alarm should repeat on. An empty array indicates the alarm should occur one time only. 

## Alarm
This class manages an alarm. Note that this class implements the IAlarm interface. 

### Properties
`Properties` - `Dictionary<string,object>` - Dictionary of additional properties of the alarm

`IsOneTime` - `Boolean` - Returns true if the alarm is a one-time alarm (`DaysOfWeek` is empty). 

### Methods
`NextInstance` - `DateTime` - Returns the next UTC instance of the alarm. 

`LastInstance` - `DateTime` - Returns the last UTC instance of the alarm. 

`ScheduleAsync` - `Task<void>` - Executes the provided action or task at the next instance of the alarm. 