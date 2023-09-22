using Mype.Common;
using Mype.Mqtt;
using Mype.Mqtt.Managed;
using MQTTnet;

namespace Master;

class LocalController {
    readonly MqttConfig _mqtt = ConfigManager<MqttConfig>.Instance.Config;
    readonly ManagedSubscriber _subscriber;
    public LocalController() {
        _subscriber = new(_mqtt);
        _subscriber.Handlers["log"] = Log;
        _subscriber.StartAsync();
    }

    private Task Log(MqttApplicationMessage message) {
        Console.WriteLine(message.Payload.GetString());
        return Task.CompletedTask;
    }
}
