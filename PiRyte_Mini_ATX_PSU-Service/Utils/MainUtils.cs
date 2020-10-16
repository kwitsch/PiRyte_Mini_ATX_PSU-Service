using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace PiRyte_Mini_ATX_PSU_Service.Utils
{
    internal class MainUtils
    {
        /// <summary>
        /// pin 18
        /// </summary>
        public readonly int Shutdown_GPIO = 24;
        /// <summary>
        /// pin 16
        /// </summary>
        public readonly int BootOk_GPIO = 23;

        private readonly ILogger<MainUtils> _Logger;

        public MainUtils(ILogger<MainUtils> logger)
        {
            _Logger = logger;
        }

        public void Shutdown()
        {
            _Logger.LogInformation("Shutdown");
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start("sudo", "shutdown -h now");
            }
        }

        public void Reboot()
        {
            _Logger.LogInformation("Reboot");
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start("sudo", "shutdown -r now");
            }
        }
    }
}
