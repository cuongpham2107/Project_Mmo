using Mype.Common;
using Mype.ConsoleMvc;
using Mype.Mqtt;
using Mype.Mqtt.Rpc;
using Shared;
using System.Diagnostics;

namespace Master;

class DriverController {
    readonly MqttConfig mqttConfig;
    readonly RpcCommander commander;
    readonly string driverId = "3cf767f8-af49-44df-bfea-cc465f367e1e";
    readonly string key = "n7QhP8aQZKWJieX1R664xeV1DNYjf2Df";
    readonly string gologinAccessToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiI2NGEyODllMzdjYzAxMTZjYjcxMTJmYjUiLCJ0eXBlIjoiZGV2Iiwiand0aWQiOiI2NGEyYmJmMmM3Y2QwZDUwOTVhZTQxNzEifQ.bZDLKoIEAMg4ZOojttL70xvlpG4HZ51JcBZk9mCQ_NU";
    readonly string pathGologin = "C:\\Users\\trick\\AppData\\Local\\Temp\\GoLogin\\profiles\\Gologin";
    readonly string pathWorker = @"C:\Users\trick\source\repos\Mmo\Worker\bin\Debug\net7.0\Worker.exe";
    readonly string pathSlave = @"C:\Users\trick\source\repos\Mmo\Worker\bin\Debug\net7.0\Slave.exe";

    public DriverController() {
        mqttConfig = new();
        commander = new(mqttConfig);
        commander.StartCommanding();
    }

    [Route("authenticate")]
    public void Authenticate() {
        var topic = $"remote\\authenticate\\{driverId}";
        var response = commander.RpcCallStringWithResponse(topic, key);
        Console.WriteLine(response.Result.Message);
    }

    [Route("setconfig")]
    public void SetConfig() {
        var topic = $"{key}\\remote\\setconfig\\{driverId}";
        var config = new Configuration() {
            GologinAccessToken = gologinAccessToken,
            PathGologin = pathGologin,
            PathWorker = pathWorker,
            PathSlave = pathSlave
        };
        var response2 = commander.RpcCallObjectWithResponse(topic, config);
        Console.WriteLine(response2.Result.Message);
    }

    [Route("ping")]
    public void Ping() {
        var topic = $"{key}\\remote\\ping\\{driverId}";
        try {
            Stopwatch sw = new();
            sw.Start();
            var response = commander.RpcCallNoPayloadWithResponse(topic);
            sw.Stop();
            Console.WriteLine($"{response.Result.Message}. Resoponsed in {sw.ElapsedMilliseconds}ms ({sw.ElapsedTicks} ticks)");
        } catch (Exception) {
            Console.WriteLine("The driver does not response");
        }
    }

    [Route("startprofile")]
    public void StartProfile() {
        var topic = $"{key}\\remote\\worker\\startprofile\\{driverId}";
        var response = commander.RpcCallNoPayloadWithResponse(topic);
        Console.WriteLine(response.Result.Message);
    }
    /// <summary>
    /// Tham số gửi đi là số profile cần tạo
    /// </summary>
    /// <param name="p"></param>
    [Route("newProfile")]
    public void Run()
    {
        List<Proxy> proxyList = new List<Proxy>
        {
            new Proxy { Host = "74.81.51.211", Port = 3200, UserName = "8dpg07of", Password = "qyoVaJZICDi2" },
            //new Proxy { Host = "74.81.51.187", Port = 3195, UserName = "zxuyz1h3", Password = "NzjbulaLvOGZ" },
        };
        string payload = proxyList.ToBase64();
        var topic = $"{key}\\remote\\worker\\startprofile\\{driverId}";
        var response = commander.RpcCallStringWithResponse(topic, payload, 5);
        Console.WriteLine(response.Result.Message);
    
    }
}
public class Proxy
{
    public string Host { get; set; }
    public int Port { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
}
