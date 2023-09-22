using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;

class Program {
    static HttpClient client = new HttpClient();

    static async Task Main(string[] args) {
        Console.OutputEncoding = Encoding.Unicode;
        string accessToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiI2NDllMzE3MzUyZDdhZWY2ZmFkOWQ3YjciLCJ0eXBlIjoiZGV2Iiwiand0aWQiOiI2NDlmODY0NTYzNTEyMmExOWJiZmE0M2MifQ._mp29fpkfWy0ed9pb17URK1ym1terWa2Wktpg7rstmA";

        // Cấu hình HttpClient và header Authorization
        client.BaseAddress = new Uri("https://api.gologin.com/browser/v2");
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        while (true) {
            try {
                // Gửi yêu cầu API để lấy danh sách profile
                HttpResponseMessage r1 = await client.GetAsync("");
                if (r1.IsSuccessStatusCode) {
                    // Đọc và hiển thị danh sách profile
                    string responseContent = await r1.Content.ReadAsStringAsync();

                    RootObject data = JsonConvert.DeserializeObject<RootObject>(responseContent);

                    Console.WriteLine("Number of profiles: " + data.Profiles.Count);
                    Console.WriteLine("Current Orbita Major Version: " + data.CurrentOrbitaMajorV);
                    Console.WriteLine("Current Browser Version: " + data.CurrentBrowserV);
                    Console.WriteLine("---------------------------------------");

                    string apiUrl = "http://localhost:36912/browser/";
                    HttpClient c = new HttpClient();
                    c.BaseAddress = new Uri(apiUrl);
                    // Đặt header Content-Type
                    c.DefaultRequestHeaders.Clear();
                    c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    // Access individual profiles
                    foreach (Profile profile in data.Profiles) {
                        Console.WriteLine("Profile Name: " + profile.name);
                        Console.WriteLine("Profile Role: " + profile.role);
                        Console.WriteLine("Profile ID: " + profile.id);
                        Console.WriteLine("Profile Browser Type: " + profile.browserType);
                        Console.WriteLine("Navigation -> Resolution: {0}", profile.navigator.resolution);

                        Console.WriteLine("Starting profile ...");

                        var p = new { profileId = profile.id, sync = true };

                        try {
                            // Gửi truy vấn POST
                            JsonContent content = JsonContent.Create(p);
                            HttpRequestMessage request = new(HttpMethod.Post, "http://localhost:36912/browser/start-profile") {
                                Content = content,                                
                            };
                            
                            HttpResponseMessage response = await client.SendAsync(request); //await c.PostAsync("start-profile", content);

                            // Kiểm tra mã trạng thái của phản hồi
                            if (response.IsSuccessStatusCode) {
                                Console.WriteLine("Truy vấn POST thành công!");
                            } else {
                                Console.WriteLine("Truy vấn POST không thành công. Mã trạng thái: " + response.StatusCode);
                            }
                        } catch (Exception ex) {
                            Console.WriteLine("Đã xảy ra lỗi khi gửi truy vấn POST: " + ex.Message);
                        }

                        Console.WriteLine("---------------------------------------");
                    }

                } else {
                    Console.WriteLine($"Lỗi: {r1.StatusCode}");
                }
            } catch (Exception e) {
                Console.WriteLine($"Lỗi: {e.Message}");
            }

            Console.ReadLine();
            Console.Clear();
        }
    }
}

public class RootObject {
    public List<Profile> Profiles { get; set; }
    public int AllProfilesCount { get; set; }
    public string CurrentOrbitaMajorV { get; set; }
    public string CurrentBrowserV { get; set; }
    public string CurrentTestBrowserV { get; set; }
    public string CurrentTestOrbitaMajorV { get; set; }
    public bool IsFolderDeleted { get; set; }
}

public class Profile {
    public string name { get; set; }
    public string role { get; set; }
    public string id { get; set; }
    public string notes { get; set; }
    public string browserType { get; set; }
    public bool lockEnabled { get; set; }
    public Timezone timezone { get; set; }
    public Navigator navigator { get; set; }
    public Geolocation geolocation { get; set; }
    public bool canBeRunning { get; set; }
    public object runDisabledReason { get; set; }
    public string os { get; set; }
    public Proxy proxy { get; set; }
    public string proxyRegion { get; set; }
    public List<object> sharedEmails { get; set; }
    public string shareId { get; set; }
    public DateTime createdAt { get; set; }
    public DateTime updatedAt { get; set; }
    public DateTime lastActivity { get; set; }
    public List<object> chromeExtensions { get; set; }
    public List<object> userChromeExtensions { get; set; }
    public List<object> tags { get; set; }
    public bool proxyEnabled { get; set; }
    public bool isBookmarksSynced { get; set; }
    public bool autoLang { get; set; }
}

public class Timezone {
    public bool fillBasedOnIp { get; set; }
    public string timezone { get; set; }
}

public class Navigator {
    public string userAgent { get; set; }
    public string resolution { get; set; }
    public string language { get; set; }
}

public class Geolocation {
    public string mode { get; set; }
    public bool enabled { get; set; }
    public bool customize { get; set; }
    public bool fillBasedOnIp { get; set; }
    public int latitude { get; set; }
    public int longitude { get; set; }
    public int accuracy { get; set; }
    public bool isCustomCoordinates { get; set; }
}

public class Proxy {
    public string mode { get; set; }
    public int port { get; set; }
    public string host { get; set; }
    public string username { get; set; }
    public string password { get; set; }
    public string autoProxyRegion { get; set; }
    public string torProxyRegion { get; set; }
    public string id { get; set; }
    public string changeIpUrl { get; set; }
    public string proxyType { get; set; }
}

