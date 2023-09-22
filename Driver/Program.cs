using Mype.Mqtt;
using Mype.Mqtt.Managed;
using Mype.Mqtt.Rpc;
using Shared;

namespace Driver;

public class Program {
    public static async Task Main(string[] args) {
        Console.WriteLine("Driver run!");
        await Mqtt.RunAsync();
        Thread.Sleep(-1);
    }
}

public class Mqtt {
    static ManagedPublisher publisher;
    static RpcListener listener;
    static readonly Configuration config = Settings.Instance.Configuration;
    static readonly string driverId = Settings.Instance.GetDriverId();

    public static ManagedPublisher Publisher => publisher;
    public static RpcListener Listener => listener;

    public static async Task RunAsync() {

        var client = ManagedClientBase.CreateManagedMqttClient();
        MqttConfig mqtt = new() { Uri = config.MqttUri, Port = config.MqttPort };
        publisher = new(client, mqtt);

        var key = Settings.Instance.GetKey();
        Console.WriteLine($"key: {key}, driverId: {driverId}");
        listener = new(mqtt, client) { Suffix = $"\\{driverId}", Prefix = $"{key}\\" };

        listener.AddController<DriverController>();
        listener.AddController<SlaveController>();
        listener.AddController<WorkerController>();

        await listener.StartListening();
        await publisher.StartAsync();
    }

    public static void Log(string message) {
        publisher.PublishStringAsync("log", $"[{driverId}] {message}");
    }
}