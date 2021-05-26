using System;

namespace HueSundowner.Lib {
  public class HueSchedule {
    public int SunsetOffsetOn_m { get; set; }
    public int SunsetOffsetOff_m { get; set; }
    public string DayOfWeekFilter { get; set; }
    public int CheckFrequency_m { get; set; }    
  }
}
