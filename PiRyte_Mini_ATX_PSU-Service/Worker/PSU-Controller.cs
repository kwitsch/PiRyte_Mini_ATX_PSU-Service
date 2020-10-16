using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PiRyte_Mini_ATX_PSU_Service.Utils;
using System;
using System.Device.Gpio;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace PiRyte_Mini_ATX_PSU_Service.Worker
{
    class PSU_Controller : BackgroundService
    {
        private readonly ILogger<PSU_Controller> _Logger;
        private readonly MainUtils _Utils;
        private readonly GpioController _Controller;
        private bool _Initialized;

        public PSU_Controller(ILogger<PSU_Controller> logger, MainUtils utils)
        {
            _Logger = logger;
            _Utils = utils;
            _Controller = new GpioController();
            _Initialized = false;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            _Logger.LogInformation("Start PiRyte_Mini_ATX_PSU_Service");
            try
            {
                _Controller.OpenPin(_Utils.Shutdown_GPIO, PinMode.InputPullDown);
                _Controller.OpenPin(_Utils.BootOk_GPIO, PinMode.Output);
                _Controller.Write(_Utils.BootOk_GPIO, PinValue.High);
                _Initialized = true;
                _Logger.LogInformation("GPIO setup complete...");
                await base.StartAsync(cancellationToken);
            }
            catch
            {
                _Logger.LogError("GPIO setup error.");
                _Logger.LogWarning("Wrong hardware?");
                await StopAsync(cancellationToken);
            }
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while(_Initialized && !cancellationToken.IsCancellationRequested)
            {
                _Controller.WaitForEvent(_Utils.Shutdown_GPIO, PinEventTypes.Rising, cancellationToken);
                _Logger.LogInformation("GPIO rising detected");

                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();
                var evres = _Controller.WaitForEvent(_Utils.Shutdown_GPIO, PinEventTypes.Falling, new TimeSpan(600));
                stopWatch.Stop();

                if (evres.EventTypes == PinEventTypes.None)
                {
                    _Controller.Write(_Utils.BootOk_GPIO, PinValue.Low);
                    _Utils.Shutdown();
                    break;
                }
                else if (stopWatch.ElapsedTicks >= 200)
                {
                    _Controller.Write(_Utils.BootOk_GPIO, PinValue.Low);
                    _Utils.Reboot();
                    break;
                }

                if (_Controller.Read(_Utils.Shutdown_GPIO) == PinValue.High)
                {
                    _Controller.WaitForEvent(_Utils.Shutdown_GPIO, PinEventTypes.Falling, cancellationToken);
                }
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _Logger.LogInformation("Stop PiRyte_Mini_ATX_PSU_Service");
            if (_Initialized)
            {
                _Controller.Write(_Utils.BootOk_GPIO, PinValue.Low);
                _Controller.Dispose();
            }
            await base.StopAsync(cancellationToken);
        }
    }
}
