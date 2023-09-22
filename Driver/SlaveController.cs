using Mype.Common;
using Mype.Mqtt;
using Mype.Mqtt.Rpc;
using Shared;
using System.Diagnostics;
using System.Management;

namespace Driver;

public class SlaveController : RpcBaseController {
    readonly Configuration _config = Settings.Instance.Configuration;
    readonly string _key = Settings.Instance.GetKey();
    
    [Topic("remote\\slave\\run", hasSuffix: true, hasPrefix: true)]
    public RpcResponse RunSlave(byte[] payload)
    {
        string data = payload.GetString();
        string newData = data.Replace("=", ".");
        ProcessStartInfo psi = new()
        {
            FileName = $"{_config.GetPathSlave()}",
            //FileName = @"C:\Mmo\Slave\bin\Debug\net7.0\Slave.exe",
            Arguments = $"run?data={newData}",
            UseShellExecute = false,
        };
        try
        {
            var p = Process.Start(psi);
            if (p != null)
            {
                p.EnableRaisingEvents = true;
                p.Exited += (s, e) =>
                {
                    Mqtt.Log($"{p.Id} Slave stopped");
                };
                return OkResponse($"{p.Id}");
            }
            else return ErrorResponse($"Start failed");

        }
        catch (Exception e)
        {
            return ErrorResponse($"Start failed: {e.Message}");
        }
    }
    
    [Topic("remote\\slave\\kill",hasSuffix:true, hasPrefix: true)]
    public RpcResponse KillSlave(byte[] payload)
    {
        int ProcessId = int.Parse(payload.GetString());
        try
        {
            Process process = Process.GetProcessById(ProcessId);
            KillAllChildProcesses(process);
            if (!process.HasExited)
            {
                process.Kill();
                return OkResponse($"Kill slave {process} successfully");
            }
            else
            {
                return OkResponse("The process was not found or ended earlier.");
            }
          
        }
        catch (ArgumentException)
        {
            return OkResponse("The process with the entered ID could not be found.");
        }
        catch (Exception ex)
        {
            return OkResponse("Something went wrong: " + ex.Message);
        }
    }
    // Đệ quy dừng tất cả các tiến trình con của process hiện tại
    private void KillAllChildProcesses(Process process)
    {
        foreach (var childProcess in GetChildProcesses(process.Id))
        {
            KillAllChildProcesses(childProcess);
            childProcess.Kill();
            childProcess.WaitForExit();
        }
    }

    // Lấy danh sách các tiến trình con của một tiến trình cha bằng cách sử dụng WMI
    private  Process[] GetChildProcesses(int parentProcessId)
    {
        ManagementObjectSearcher childProcesses = new ManagementObjectSearcher($"SELECT * FROM Win32_Process WHERE ParentProcessId={parentProcessId}");
        var childProcessList = childProcesses.Get();
        var childList = new Process[childProcessList.Count];

        int index = 0;
        foreach (ManagementObject child in childProcessList)
        {
            int childProcessId = Convert.ToInt32(child["ProcessId"]);
            childList[index++] = Process.GetProcessById(childProcessId);
        }

        return childList;
    }

}
