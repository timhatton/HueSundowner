using System;

namespace HueSundowner.Lib {
  public interface IClock {
    DateTime GetNow();
  }
}