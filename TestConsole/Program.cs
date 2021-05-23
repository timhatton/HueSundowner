using HueSundowner.Lib;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

namespace TestConsole {
  class Program {
    static async Task Main(string[] args) {    
      if(args.Length != 1 || (args[0].ToLower() != "on" && args[0].ToLower() != "off")) {
        Console.WriteLine("test [on | off]");
        Environment.Exit(1);
      }

      var builder = new ConfigurationBuilder()
                 .SetBasePath(Directory.GetCurrentDirectory())
                 .AddJsonFile("appSettings.json");

      var configuration = builder.Build();

      var settings = configuration.GetSection("HueSettings").Get<HueSettings>();

      Console.WriteLine($"Hue bridge at {settings.IpAddress}");
      var controller = new HueController(settings.IpAddress, settings.AppKey);

      if(args[0].ToLower() == "on") {
        await controller.On();
      }
      else if(args[0].ToLower() == "off") {
        await controller.Off();
      }
    }
  }
}
