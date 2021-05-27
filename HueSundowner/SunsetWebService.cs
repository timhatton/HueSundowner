using Newtonsoft.Json;
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
     
        using(var response = await httpClient.GetAsync(string.Format(ServiceUrlFormat, latitude, longitude, date.ToIso8601()))) {
          string apiResponse = await response.Content.ReadAsStringAsync();
          var ssResponse = JsonConvert.DeserializeObject<SunriseSunsetResponse>(apiResponse);

          return ssResponse.Results.Sunset;
        }     
    }
    
    double latitude;
    double longitude;
    HttpClient httpClient;
  }
}
