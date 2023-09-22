namespace Slave
{
    public class Data
    {
       
        public string ProfileId { get; set; }
        public int PointX { get; set; } 
        public int PointY { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int TimeToWatchVideo { get; set; }
        public string[] Keywords { get; set; }
        public string[] Channels { get; set; }
        public string[] Urls { get; set; }
        public string[] Comments { get; set; }
        public string[] Icons { get; set; }
        public int Script { get; set; }
        public Data()
        {

        }
       

        public Data(string profileId, string email, string password, int timeToWatchVideo, string[] keywords, string[] channels, string[] urls, string[] comments, string[] icons, int script)
        {
            ProfileId = profileId;
            Email = email;
            Password = password;
            TimeToWatchVideo = timeToWatchVideo;
            Keywords = keywords;
            Channels = channels;
            Urls = urls;
            Comments = comments;
            Icons = icons;
            Script = script;
        }
    }
    public class GmailUpdate
    {
        public string email { get; set; }
        public string password { get; set; }
    }
}