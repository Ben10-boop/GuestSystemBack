using GuestSystemBack.Interfaces;
using System.Drawing.Text;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using GuestSystemBack.DTOs;
using System.Text;

namespace GuestSystemBack.Services
{
    public class CiscoApiService : ICiscoApiService
    {
        private readonly IConfiguration _configuration;
        public CiscoApiService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public List<GuestUser> GetCurrentWifiUsers()
        {
            using(var client = new HttpClient())
            {
                var jsonSettings = new JsonSerializerSettings();
                jsonSettings.MissingMemberHandling = MissingMemberHandling.Ignore;
                List<string> itemLinks;
                using (var request = new HttpRequestMessage(HttpMethod.Get, _configuration.GetSection("AppSettings:CiscoURL").Value))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Basic", _configuration.GetSection("AppSettings:CiscoPassword").Value);
                    request.Headers.CacheControl = new CacheControlHeaderValue
                    {
                        NoCache = true
                    };
                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var result = client.SendAsync(request).Result;
                    var json = result.Content.ReadAsStringAsync().Result;
                    string[] values = json.Split('"');
                    itemLinks = values.Where(o => o.StartsWith("https://")).ToList();
                }

                List<GuestUser> guestAccDetailList = new List<GuestUser>();
                foreach (var itemLink in itemLinks)
                {
                    using (var request = new HttpRequestMessage(HttpMethod.Get, itemLink))
                    {
                        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", _configuration.GetSection("AppSettings:CiscoPassword").Value);
                        request.Headers.CacheControl = new CacheControlHeaderValue
                        {
                            NoCache = true
                        };
                        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        var result = client.SendAsync(request).Result;
                        var json = result.Content.ReadAsStringAsync().Result;
                        var wifiGuest = JsonConvert.DeserializeObject<GuestUserDTO>(json, jsonSettings).GuestUser;
                        if(wifiGuest.status == "ACTIVE")
                            guestAccDetailList.Add(wifiGuest);
                    }
                }
                return guestAccDetailList;
            }
        }

        public void PostWifiUser(GuestUserDTO guestUser)
        {
            using (var client = new HttpClient())
            {
                using (var request = new HttpRequestMessage(HttpMethod.Post, _configuration.GetSection("AppSettings:CiscoURL").Value))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Basic", _configuration.GetSection("AppSettings:CiscoPassword").Value);
                    request.Headers.CacheControl = new CacheControlHeaderValue
                    {
                        NoCache = true
                    };
                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var postContent = JsonConvert.SerializeObject(guestUser);
                    var postPayload = new StringContent(postContent, Encoding.UTF8, "application/json");
                    request.Content = postPayload;
                    var result = client.SendAsync(request).Result;
                }
            }
        }
    }
}
