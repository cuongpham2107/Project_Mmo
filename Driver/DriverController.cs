using Mype.Common;
using Mype.Mqtt;
using Mype.Mqtt.Rpc;
using Shared;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace Driver;
public class DriverController : RpcBaseController {

    [Topic("remote\\ping", hasPrefix: true, hasSuffix: true)]
    public RpcResponse Ping() {
        return OkResponse("Running");
    }

    [Topic("remote\\setconfig", hasPrefix: true, hasSuffix: true)]
    public RpcResponse SetConfig(byte[] payload) {
        var config = payload.GetString();
        if (Settings.Instance.CreateConfig(config)) {
            Settings.Instance.SaveFile();
            return OkResponse();
        }
        return ErrorResponse("Json data corrupted");
    }
    
    [Topic("remote\\authenticate", hasSuffix: true)]
    public RpcResponse Authenticate(byte[] payload) {
        var key = payload.GetString();
        var localkey = Settings.Instance.GetKey();
        if (key == localkey) return OkResponse();

        if (Settings.Instance.SetKey(key)) {
            var name = Environment.MachineName;
            var host = Dns.GetHostEntry(Dns.GetHostName());

            StringBuilder sb = new();

            foreach (var ip in host.AddressList) {
                if (ip.AddressFamily == AddressFamily.InterNetwork) {
                    sb.AppendFormat("{0}, ", ip);
                }
            }
            var ips = sb.ToString().Trim(new[] { ',', ' ' });
            var info = new {
                Name = name,
                IPs = ips
            };
            //restart mqtt to acce
            Mqtt.RunAsync().Wait();
            return OkResponse(JsonSerializer.Serialize(info));
        } else return ErrorResponse("Cannot set key");
    }    
}
