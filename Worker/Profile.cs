using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Worker;
public class RootObject
{
    public List<Profile> Profiles { get; set; }
    public int AllProfilesCount { get; set; }
    public string CurrentOrbitaMajorV { get; set; }
    public string CurrentBrowserV { get; set; }
    public string CurrentTestBrowserV { get; set; }
    public string CurrentTestOrbitaMajorV { get; set; }
    public bool IsFolderDeleted { get; set; }
}

public class Profile
{
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

public class Timezone
{
    public bool fillBasedOnIp { get; set; }
    public string timezone { get; set; }
}

public class Navigator
{
    public string userAgent { get; set; }
    public string resolution { get; set; }
    public string language { get; set; }
}

public class Geolocation
{
    public string mode { get; set; }
    public bool enabled { get; set; }
    public bool customize { get; set; }
    public bool fillBasedOnIp { get; set; }
    public int latitude { get; set; }
    public int longitude { get; set; }
    public int accuracy { get; set; }
    public bool isCustomCoordinates { get; set; }
}

public class Proxy
{
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
public class DataResponse
{
    public int CountProfile { get; set; }
    public List<ProxyProfile> ListProxy { get; set; }
}
public class ProxyProfile
{
    public string Oid { get;set; }
    public string Driver { get; set; }
    public string Host { get; set; }
    public int Port { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
}
