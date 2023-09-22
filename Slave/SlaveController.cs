using Mype.ConsoleMvc;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Mype.Common;
using Shared;

namespace Slave
{
    public class SlaveController
    {
        readonly Configuration _config = Settings.Instance.Configuration;
        public SlaveController()
        {
        
        }
        [Route("run")]
        public void Run(Dictionary<string,string> pairs) 
        {
           
            string resData = pairs["data"].Replace(".", "=");
            Data data = resData.FromBase64To<Data>();
            ChromeOptions options = new ChromeOptions();
            options.BinaryLocation = _config.GetPathOrbita();
            options.AddArgument($"--user-data-dir={_config.GetPathProfile(data.ProfileId)}");
            options.AddArgument($"--load-extension={_config.GetPathCookieExtenstion(data.ProfileId)},{_config.GetPathPasswordExtension(data.ProfileId)},{_config.GetPathProxyExtension(data.ProfileId)}");
            options.AddArguments(new[] { "--font-masking-mode=2", "--profile-directory=Default", "--disable-encryption", "--donut-pie=undefined", "--lang=en-US", "--flag-switches-begin", "--flag-switches-end" });
            IWebDriver driver = new ChromeDriver(options);
            driver.Manage().Window.Position = new System.Drawing.Point(data.PointX, data.PointY);
            driver.Manage().Window.Size = new System.Drawing.Size(450, 450);
            // Tạo đối tượng Context và đặt các giá trị cần thiết vào Data
            var context = new Context(driver, new Data(data.ProfileId, data.Email, data.Password, data.TimeToWatchVideo, data.Keywords, data.Channels, data.Urls, data.Comments, data.Icons,data.Script));
            //context.ExecuteState();
            while (true)
            {
                context.ExecuteState().Wait();
            }
        }
    }
}
