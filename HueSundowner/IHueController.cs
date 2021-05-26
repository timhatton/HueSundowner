using System.Threading.Tasks;

namespace HueSundowner.Lib {
  public interface IHueController {
    Task Off();
    Task On();
  }
}