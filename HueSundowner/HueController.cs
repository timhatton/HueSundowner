using Q42.HueApi;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HueSundowner.Lib {
  public class HueController: IHueController {
    public HueController(string ipAddress, string appKey) {
      this.ipAddress = ipAddress;
      this.appKey = appKey;
    }
    public async Task On() {
      try {
        logger.Information("Turning lights on");
        LocalHueClient client = new LocalHueClient(ipAddress);
        client.Initialize(appKey);
        var command = new LightCommand();
        command.On = true;

        await client.SendCommandAsync(command);
      }
      catch(Exception ex) {
        logger.Information("Turning lights on");
        logger.Information($"Error turning lights on: {ex.Message}");
      }
    }
    public async Task Off() {
      try {
        logger.Information("Turning lights off");
        LocalHueClient client = new LocalHueClient(ipAddress);
        client.Initialize(appKey);
        var command = new LightCommand();
        command.On = false;

        await client.SendCommandAsync(command);
      }
      catch(Exception ex) {
        logger.Information("Turning lights off");
        logger.Information($"Error turning lights off: {ex.Message}");
      }
    }
    string ipAddress;
    string appKey;
    private static readonly ILogger logger = Log.Logger.ForContext<HueController>();
  }
}
