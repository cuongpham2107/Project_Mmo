using Mype.Common;
using Mype.ConsoleMvc;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Text;

namespace Slave;

internal class Program {
    static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        Application application = new();
        application.AddController<SlaveController>();
        //application.Run();
        application.RunHeadless(args[0]);
       
        
    }

}
