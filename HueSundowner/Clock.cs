using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HueSundowner.Lib {
  public class Clock: IClock {
    public DateTime GetNow() {
      return DateTime.UtcNow;
    }
  }
}
