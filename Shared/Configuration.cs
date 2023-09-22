using System.IO;
using System.Text.Json;

namespace Shared;

public class Configuration {
    public string DriverId { get; set; } = "";
    public string GologinAccessToken { get; set; } = "";
    public string DefaultWorkingPath { get; set; } = "";
    public string PathGologin { get; set; } = "{DefaultWorkingPath}\\Gologin";
    public string ApiBaseUrl { get; set; } = "https://app.kennatech.vn/mmo-api/api";
    public string ApiCreateProfile { get; set; } = "https://api.gologin.com/browser";
    public string ApiUpdateProfile { get; set; } = "https://api.gologin.com/browser/fingerprint/multi";
    public string ApiStartProfile { get; set; } = "http://localhost:36912/browser/start-profile";
    public string ApiDeleteProfile { get; set; } = "https://api.gologin.com/browser/{id}";

    public string PathOrbita { get; set; } = "{PathGologin}\\Browser\\orbita-browser-115\\chrome.exe";
    public string PathProfile { get; set; } = "{PathGologin}\\Profiles\\{id}";
    public string PathCookieExtension { get; set; } = "{PathGologin}\\Extensions\\cookies-ext\\{id}";
    public string PathPasswordExtension { get; set; } = "{PathGologin}\\Extensions\\passwords-ext\\{id}";
    public string PathProxyExtension { get; set; } = "{PathGologin}\\Extensions\\chrome-extensions\\{id}";

    public string PathSlave { get; set; } = "{DefaultWorkingPath}\\Release\\Slave\\Slave.exe";
    public string PathWorker { get; set; } = "{DefaultWorkingPath}\\Release\\Worker\\Worker.exe";    

    public string ChromeCommandLine { get; set; } = @"--user-data-dir=""{PathProfile}"" --disable-encryption --donut-pie=undefined --font-masking-mode=2 --load-extension=""{PathCookieExtension},{PathPasswordExtension},{PathProxyExtension}"" --lang=en-US --restore-last-session --flag-switches-begin --flag-switches-end";
    public string[] Arguments { get; set; } = new[] { "--font-masking-mode=2", "--profile-directory=Default", "--disable-encryption", "--donut-pie=undefined", "--lang=en-US", "--flag-switches-begin", "--flag-switches-end" };

    public string MqttUri { get; set; } = "app.kennatech.vn";
    public int MqttPort { get; set; } = 1883;

    public string GetApiDeleteProfile(string id) => ApiDeleteProfile.Replace("{id}", id);
    public string GetPathGoLogin() => PathGologin.Replace("{DefaultWorkingPath}", DefaultWorkingPath);
    public string GetPathOrbita() => PathOrbita.Replace("{PathGologin}", GetPathGoLogin());
    public string GetPathProfile(string id) => PathProfile.Replace("{PathGologin}", GetPathGoLogin()).Replace("{id}", id);
    public string GetPathCookieExtenstion(string id) => PathCookieExtension.Replace("{PathGologin}", GetPathGoLogin()).Replace("{id}", id);
    public string GetPathPasswordExtension(string id) => PathPasswordExtension.Replace("{PathGologin}", GetPathGoLogin()).Replace("{id}", id);
    public string GetPathProxyExtension(string id) => PathProxyExtension.Replace("{PathGologin}", GetPathGoLogin()).Replace("{id}", id);
    public string GetChromeCommandLine(string id) => ChromeCommandLine
        .Replace("{PathProfile}", GetPathProfile(id))        
        .Replace("{PathCookieExtension}", GetPathCookieExtenstion(id))
        .Replace("{PathPasswordExtension}", GetPathPasswordExtension(id))
        .Replace("{PathProxyExtension}", GetPathProxyExtension(id));
    public string GetPathSlave() => PathSlave.Replace("{DefaultWorkingPath}", DefaultWorkingPath);
    public string GetPathWorker() => PathWorker.Replace("{DefaultWorkingPath}", DefaultWorkingPath);
}
