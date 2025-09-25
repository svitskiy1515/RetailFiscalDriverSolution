using System.ServiceProcess;
using Microsoft.Extensions.DependencyInjection;

namespace DriverWindowsService.Hosting
{
    public sealed class ServiceHost : ServiceBase
    {
        private ServiceProvider _sp;
        private Worker _worker;

        protected override void OnStart(string[] args)
        {
            _sp = Composition.CompositionRoot.BuildServiceProvider();
            _worker = _sp.GetRequiredService<Worker>();
            _worker.Start();
        }

        protected override void OnStop()
        {
            _worker?.Stop();
            _sp?.Dispose();
        }

        // режим консоли
        public void StartConsole()
        {
            _sp = Composition.CompositionRoot.BuildServiceProvider();
            _worker = _sp.GetRequiredService<Worker>();
            _worker.Start();
        }

        public void StopConsole()
        {
            _worker?.Stop();
            _sp?.Dispose();
        }
    }
}