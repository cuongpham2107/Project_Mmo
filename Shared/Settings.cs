using System.Text.Json;

namespace Shared;

public class Settings {
    static readonly Settings _instance = new();

    readonly string configPath = $"{Path.GetTempPath()}.julia";
    readonly string keyPath = Path.Combine(Path.GetTempPath(), ".key");
    private Settings() {
        if (File.Exists(configPath)) {
            try {
                var temp = JsonSerializer.Deserialize<Configuration>(File.ReadAllText(configPath));
                if (temp is not null) Configuration = temp;
                else {
                    Configuration = new();
                    SaveFile();
                }
            } catch (Exception) {
                SaveFile();
            }
        } else {
            Configuration = new();
            SaveFile();
        }
    }

    public bool CreateConfig(string json) {
        var temp = JsonSerializer.Deserialize<Configuration>(json);
        if (temp is not null) {
            Configuration = temp;
            return true;
        }
        return false;
    }

    public void SaveFile() {
        var json = JsonSerializer.Serialize(Configuration);
        File.WriteAllText(configPath, json);
    }

    public static Settings Instance => _instance ?? new();

    public Configuration Configuration { get; set; }

    public string GetKey() {
        if (File.Exists(keyPath)) {
            var sr = File.OpenText(keyPath);
            var key = sr.ReadToEnd();
            sr.Close();
            return key;
        } else {
            return string.Empty;
        }
    }

    public bool SetKey(string key) {
        try {
            File.WriteAllText(keyPath, key);
            return true;
        } catch (Exception) {
            return false;
        }
    }

    public string GetDriverId() {
        var path = Path.GetDirectoryName(Environment.ProcessPath);
        var idfile = Path.Combine(path, ".driverid");
        if (File.Exists(idfile))    
            return File.ReadAllText(idfile).Trim();
        else return string.Empty;
    }
}