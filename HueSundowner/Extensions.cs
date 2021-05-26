using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HueSundowner.Lib {
  public static class Extensions {
   
    public static bool IsAm(this DateTime dateTime) {
      return dateTime < dateTime.Date.AddHours(12);
    }
    public static string ToIso8601(this DateTime dateTime) {
      return dateTime.ToString("s", System.Globalization.CultureInfo.InvariantCulture);
    }
  }
}
