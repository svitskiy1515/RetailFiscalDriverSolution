using System;
using System.ServiceProcess;

namespace DriverTrayApp
{
    public static class ServiceManager
    {
        private const string ServiceName = "PilotFiscalDriverService";

        public static ServiceControllerStatus GetStatus()
        {
            using (var sc = new ServiceController(ServiceName))
            {
                return sc.Status;
            }
        }

        public static void Start()
        {
            using (var sc = new ServiceController(ServiceName))
            {
                if (sc.Status == ServiceControllerStatus.Stopped)
                {
                    sc.Start();
                    sc.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(30));
                }
            }
        }

        public static void Stop()
        {
            using (var sc = new ServiceController(ServiceName))
            {
                if (sc.Status == ServiceControllerStatus.Running)
                {
                    sc.Stop();
                    sc.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(30));
                }
            }
        }

        public static void Restart()
        {
            Stop();
            Start();
        }
    }
}