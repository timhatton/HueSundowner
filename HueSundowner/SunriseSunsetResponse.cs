using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HueSundowner.Lib {
  public class SunriseSunsetResponse {
    public SunriseSunsetResult Results { get; set; }
    public string Status { get; set; }
  }
  public class SunriseSunsetResult {
    public DateTime Sunrise { get; set; }
    public DateTime Sunset { get; set; }
    public DateTime Solar_noon { get; set; }
    public int Day_length { get; set; }
    public DateTime Civil_twilight_begin { get; set; }
    public DateTime Civil_twilight_end { get; set; }
    public DateTime Nautical_twilight_begin { get; set; }
    public DateTime Nautical_twilight_end { get; set; }
    public DateTime Astronomical_twilight_begin { get; set; }
    public DateTime Astronomical_twilight_end { get; set; }
  }
}
