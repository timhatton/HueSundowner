using System;
using System.Threading.Tasks;

namespace HueSundowner.Lib {
  public interface ISunsetWebService {
    Task<DateTime> GetSundownTimeAsLocal(DateTime date);
  }
}