using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PiRyte_Mini_ATX_PSU_Service.Utils;
using PiRyte_Mini_ATX_PSU_Service.Worker;

namespace PiRyte_Mini_ATX_PSU_Service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton<MainUtils>();
                    services.AddHostedService<PSU_Controller>();
                });
    }
}
