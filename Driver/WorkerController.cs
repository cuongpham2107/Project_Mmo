using Mype.Common;
using Mype.Mqtt;
using Mype.Mqtt.Rpc;
using Shared;
using System;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Driver;

class WorkerController : RpcBaseController {
    readonly Configuration _config = Settings.Instance.Configuration;
    readonly string _key = Settings.Instance.GetKey();

    [Topic("remote\\worker\\startprofile", hasPrefix: true, hasSuffix: true)]
    public RpcResponse RunWorker(byte[] payload) {

        string listProxy = payload.GetString();
        string newListProxy = listProxy.Replace("=", ".");
        
        ProcessStartInfo psi = new()
        {
            FileName = $"{_config.GetPathWorker()}",
            //FileName = @"C:\Mmo\Worker\bin\Debug\net7.0\Worker.exe",
            Arguments = $"new?proxy={newListProxy}",
            UseShellExecute = false // Đặt thành false để chạy headless

        };
        try
        {
            var p = Process.Start(psi);
            if (p != null)
            {
                p.EnableRaisingEvents = true;
                
                p.Exited += (s, e) =>
                {
                };
                return OkResponse("Đang tạo profile !!!");
            }
            else return ErrorResponse("Start worker failed");
        }
        catch (Exception ex)
        {
            return ErrorResponse($"Start worker failed: {ex.Message}");
        }
        return OkResponse();
    }
}