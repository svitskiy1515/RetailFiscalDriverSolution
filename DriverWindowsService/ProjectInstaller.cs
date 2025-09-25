using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace DriverWindowsService
{
    [RunInstaller(true)]
    public class ProjectInstaller : Installer
    {
        public ProjectInstaller()
        {
            var process = new ServiceProcessInstaller
            {
                Account = ServiceAccount.LocalSystem
            };
            var service = new ServiceInstaller
            {
                ServiceName = "PilotFiscalDriverService",
                DisplayName = "Pilot Fiscal Driver Service",
                StartType = ServiceStartMode.Automatic
            };
            Installers.Add(process);
            Installers.Add(service);
        }
    }
}
