using System;
using System.Collections.Generic;

namespace HueSundowner.Lib {
  public class HueSchedule {
    public int SunsetOffsetOn_m { get; set; }
    public List<TimeOfDayOff> TimeOfDayOff { get; set; }
    public string DayOfWeekFilter { get; set; }
    public int CheckFrequency_m { get; set; }    
  }
  public class TimeOfDayOff {
    public string DayOfWeek { get; set; }
    public string TimeOfDay { get; set; }
  }
  public class TimeOfDay {
    public TimeOfDay() { }
    public TimeOfDay(int hour, int minute) {
      if(hour>=0 && hour<24) {
        Hour=hour;
      }
      else {
        throw new ArgumentOutOfRangeException("hour");
      }
      if(minute>=0 && minute<60) {
        Minute=minute;
      }
      else {
        throw new ArgumentOutOfRangeException("minute");
      }
    }
    public TimeOfDay(DateTime dateTime) {
      Hour=dateTime.Hour;
      Minute=dateTime.Minute;
    }
    public TimeOfDay(string timeOfDay) {
      var parts = timeOfDay.Split(new char[] { ':' });
      if(parts.Length!=2) throw new ArgumentOutOfRangeException(timeOfDay);
      if(parts.Length>0) {
        int h = 0;
        if(Int32.TryParse(parts[0], out h)) {
          if(h>=0 && h<24) {
            Hour=h;
          }
          else {
            throw new ArgumentOutOfRangeException(timeOfDay);
          }
        }
      }
      if(parts.Length>1) {
        int m = 0;
        if(Int32.TryParse(parts[1], out m)) {
          if(m>=0 && m<60) {
            Minute=m;
          }
          else {
            throw new ArgumentOutOfRangeException(timeOfDay);
          }
        }
      }
    }
    public int Hour { get; set; }
    public int Minute { get; set; }
    public override string ToString() {
      return string.Format("{0:00}:{1:00}", Hour, Minute);
    }
  }
 
}
