using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HueSundowner.Lib {
  public class SunsetWebService: ISunsetWebService {
    const string ServiceUrlFormat = "https://api.sunrise-sunset.org/json?lat={0}&lng={1}&date={2}&formatted=0";
    public SunsetWebService(HttpClient httpClient, float latitude, float longitude) {
      this.httpClient = httpClient;
      this.latitude = latitude;
      this.longitude = longitude;

    }
    public async Task<DateTime> GetSundownTimeAsLocal(DateTime date) {
      var url = string.Format(ServiceUrlFormat, latitude, longitude, date.ToString("yyyy-MM-dd"));
      logger.Debug("Sending request: {Url}", url);
      using(var response = await httpClient.GetAsync(url)) {
        string apiResponse = await response.Content.ReadAsStringAsync();
        var ssResponse = JsonConvert.DeserializeObject<SunriseSunsetResponse>(apiResponse);
        logger.Debug("Response: {@Ressponse}", ssResponse);
        return ssResponse.Results.Sunset;
      }
    }

    double latitude;
    double longitude;
    HttpClient httpClient;
    private static readonly ILogger logger = Log.Logger.ForContext<SunsetWebService>();
  }
}
