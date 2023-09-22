using Mype.Common;
using Mype.ConsoleMvc;
using Mype.Mqtt;
using Shared;
using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Mype.Mqtt.Rpc;

namespace Worker;

internal class Program {
    static void Main(string[] args) {
        Console.OutputEncoding = Encoding.UTF8;
        Application application = new();
        application.AddController<WorkerController>();
        //application.Run();
        application.RunHeadless(args[0]);
    }
}
class WorkerController {
    readonly Configuration _config = Settings.Instance.Configuration;
    readonly HttpClient client = new();
    public WorkerController()
    {
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _config.GologinAccessToken);
    }

    [Route("new")]
    public void NewProfile(Dictionary<string, string> pairs) 
    {
        string jsonData = pairs["proxy"].Replace(".","=");
      
        DataResponse data = jsonData.FromBase64To<DataResponse>();
        Console.WriteLine(data);
        int count = data.CountProfile;
        List<ProxyProfile> proxyList = data.ListProxy;
        if (proxyList?.Count > 0)
        {
            for (int j = 0; j < proxyList.Count; j++)
            {
                for (int i = 0; i < count; i++)
                {
                    Console.WriteLine($"[{i}] Creating new profile ...");
                    var id = CreateProfile(proxyList[j].Host, proxyList[j].Port, proxyList[j].Username, proxyList[j].Password).WaitAsync(CancellationToken.None);
                    Console.WriteLine(id.Result);

                    UpdateProfile(id.Result).Wait();
                    Console.WriteLine("Updated profile. Starting profile ...");

                    StartProfile(id.Result).Wait();
                    Console.WriteLine("Started chrome. Deleting online profile ...");

                    DeleteProfile(id.Result).Wait();
                    Console.WriteLine("Deleted gologin online profile");

                    //Console.Write("Press enter to continue");
                    //Console.ReadLine();

                    StopChrome().Wait();
                    Console.WriteLine("Chrome closed. Checking offline folder ...");

                    var exists = CheckTempFolder(id.Result);
                    if (exists)
                    {
                        Console.WriteLine($"An offline profile was created");
                        CopyData(id.Result,_config.GetPathGoLogin());
                        Console.WriteLine("Copied to destination folders");
                        SetProxy(id.Result, proxyList[j].Host, proxyList[j].Port, proxyList[j].Username, proxyList[j].Password);
                        Console.WriteLine("Proxy extension set");
                    }
                    else Console.WriteLine("Failed! Folder deleted");

                    PostProfileXAF(proxyList[j].Driver,proxyList[j].Oid,id.Result).Wait();
                    Console.WriteLine("-----");
                }
            }
        }
    }

    public async Task PostProfileXAF(string Driver, string OidProxy, string idProfile)
    {
        string apiUrl = $"{_config.ApiBaseUrl}/odata/Profile";
        var data = new
        {
            Driver = new
            {
                Oid = Driver,
            },
            Id = idProfile,
            Proxy = new
            {
                Oid = OidProxy,
            }
        };
        using (HttpClient client = new HttpClient())
        {
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpContent content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(data),
                                                     System.Text.Encoding.UTF8,
                                                     "application/json");
            HttpResponseMessage response = await client.PostAsync(apiUrl, content);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Dữ liệu đã được gửi thành công!");
            }
            else
            {
                Console.WriteLine("Có lỗi khi gửi dữ liệu: " + response.ReasonPhrase);
            }
        }
    }
    public async Task<string> CreateProfile(string host, int port, string username, string password)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, _config.ApiCreateProfile);
        //request.Headers.Add("Authorization", $"Bearer {_config.GologinAccessToken}");
        var json = System.IO.File.ReadAllText($"{Environment.CurrentDirectory}\\.template.json");
        var str = json.Replace("{name}", Extension.RandomString(10, false))
                .Replace("{fonts}", RandomFonts())
                .Replace("{host}", host)
                .Replace("{port}", port)
                .Replace("{username}", username)
                .Replace("{password}",password);
       
        var content = new StringContent(str, Encoding.UTF8, "application/json");
        request.Content = content;
        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        string responseBody = await response.Content.ReadAsStringAsync();
        var profile = JsonSerializer.Deserialize<Profile>(responseBody);
        //Console.WriteLine(responseBody);
        return profile.id;
    }
    public async Task UpdateProfile(string id)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, _config.ApiUpdateProfile);
        //request.Headers.Add("Authorization", $"Bearer {_config.GologinAccessToken}");
        var obj = new { instanceIds = new[] { id } };
        var json = JsonSerializer.Serialize(obj);
        var content = new StringContent(json, null, "application/json");
        request.Content = content;
        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        //Console.WriteLine(await response.Content.ReadAsStringAsync());
    }
    public async Task StartProfile(string id)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, _config.ApiStartProfile);
        var obj = new { profileId = id, sync = true };
        var content = new StringContent(JsonSerializer.Serialize(obj), null, "application/json");
        request.Content = content;
        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        //Console.WriteLine(await response.Content.ReadAsStringAsync());
    }
    public static async Task StopChrome()
    {
        // Tìm và tắt trình duyệt Google Chrome
        Process[] processes = Process.GetProcessesByName("chrome");
        foreach (Process p in processes)
        {
            if (!p.HasExited)
            {
                await Task.Run(p.CloseMainWindow);
                p.WaitForExit(2000);
                if (!p.HasExited)
                {
                    await Task.Run(p.Kill);
                }
            }
        }
    }
    public async Task DeleteProfile(string id)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, _config.ApiDeleteProfile.Replace("{id}",id));
        var content = new StringContent(string.Empty);
        request.Content = content;
        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        //Console.WriteLine(await response.Content.ReadAsStringAsync());
    }
    public static bool CheckTempFolder(string id)
    {
        var fullpath = string.Format("{0}\\GoLogin\\profiles\\{1}", Path.GetTempPath(), id);
        return Path.Exists(fullpath);
    }

    public static void CopyData(string id, string baseDir)
    {
        CopyDirectory(string.Format("{0}\\GoLogin\\profiles\\{1}", Path.GetTempPath(), id), $"{baseDir}\\Profiles\\{id}");
        CopyDirectory($"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}\\.gologin\\extensions\\cookies-ext\\{id}", $"{baseDir}\\Extensions\\cookies-ext\\{id}");
        CopyDirectory($"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}\\.gologin\\extensions\\passwords-ext\\{id}", $"{baseDir}\\Extensions\\passwords-ext\\{id}");
    }

    public static void CopyDirectory(string sourceDir, string targetDir)
    {
        try
        {
            // Lấy danh sách tất cả các tệp và thư mục con trong thư mục nguồn
            string[] files = Directory.GetFiles(sourceDir, "*", SearchOption.AllDirectories);

            // Tạo thư mục đích nếu nó chưa tồn tại
            Directory.CreateDirectory(targetDir);

            // Copy tất cả các tệp sang thư mục đích
            foreach (string file in files)
            {
                // Tạo đường dẫn đầy đủ cho tệp trong thư mục đích
                string relativePath = file.Substring(sourceDir.Length + 1);
                string targetFile = Path.Combine(targetDir, relativePath);

                // Tạo các thư mục cha cho tệp nếu chúng chưa tồn tại
                Directory.CreateDirectory(Path.GetDirectoryName(targetFile));

                // Copy tệp
                System.IO.File.Copy(file, targetFile);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
       
    }

    public static string[] GetFonts()
    {
        var path = $"{Environment.CurrentDirectory}\\.fonts.txt";
        var fonts = System.IO.File.ReadAllLines(path);
        return fonts;
    }

    public static string RandomFonts(int count = 5)
    {
        var allFonts = GetFonts();
        var r = new Random();
        var fonts = new StringBuilder();
        List<int> selected = new();
        for (int i = 0; i < count; i++)
        {
            var index = r.Next(0, allFonts.Length - 1);
            while (true)
            {
                if (selected.Contains(index)) continue;
                fonts.AppendFormat("\"{0}\",", allFonts[index]);
                break;
            }
            selected.Add(index);
        }
        return fonts.ToString().TrimEnd(',');
    }

    public void SetProxy(string id, string host = "",int port = 80, string username = "", string password = "")
    {
        var path = $"{_config.GetPathGoLogin()}\\Extensions\\chrome-extensions\\{id}";
        Directory.CreateDirectory(path);

        var template = System.IO.File.ReadAllText(".background.js")
            .Replace("{host}", host)
            .Replace("{port}", port)
            .Replace("{username}", username)
            .Replace("{password}", password);

        System.IO.File.Copy(".manifest.json", $"{path}\\manifest.json");
        System.IO.File.WriteAllText($"{path}\\background.js", template);
    }

    public static bool CheckProxy(string proxyUrl)
    {
        try
        {
            // Tạo một WebProxy với thông tin proxy đã cung cấp
            WebProxy proxy = new WebProxy(proxyUrl);

            // Cung cấp thông tin xác thực cho proxy (nếu được yêu cầu)
            proxy.Credentials = new NetworkCredential("159igkzm", "QOMjH3WZQ5sA"); // Thay thế "username" và "password" bằng thông tin xác thực thực tế

            // Tạo một WebRequest sử dụng proxy
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://www.example.com");
            request.Proxy = proxy;

            // Gửi một yêu cầu HTTP GET sử dụng proxy
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                // Kiểm tra mã trạng thái của phản hồi
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    //Console.WriteLine("Proxy is working.");
                    return true;
                }
                else
                {
                    //Console.WriteLine("Proxy is not working. Status code: " + response.StatusCode);
                    return false;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred while checking the proxy: " + ex.Message);
            return false;
        }
    }

}

