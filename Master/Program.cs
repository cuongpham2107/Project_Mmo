using Mype.Common;
using Mype.ConsoleMvc;
using Mype.Mqtt;
using Mype.Mqtt.Unmanaged;

namespace Master;

internal class Program {
    static void Main() {        
        Application application = new();
        application.AddController<DriverController>();
        application.AddController<LocalController>();
        application.Run();
    }    
}