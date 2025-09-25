//using System.ServiceModel;
//using System.ServiceProcess;
//using Serilog;

//namespace DriverWindowsService
//{
//    public class DriverWindowsService : ServiceBase
//    {
//        private ServiceHost _host;
//        private EcrDriverInitializer _driver;
//        public DriverWindowsService()
//        {
//            ServiceName = "PilotFiscalDriverService";
//        }

//        protected override void OnStart(string[] args)
//        {
//            _driver = new EcrDriverInitializer(1, 57600);
//            _driver.Init();
//            _host = new ServiceHost(typeof(RemoteDriverService));
//            _host.Open();
//        }

//        protected override void OnStop()
//        {
//            if (_host != null)
//            {
//                try { _host.Close(); } catch { _host.Abort(); }
//                _host = null;
//                _driver?.Dispose();
//                Log.Information("Сервис остановлен");
//            }
//        }
//    }
//}
