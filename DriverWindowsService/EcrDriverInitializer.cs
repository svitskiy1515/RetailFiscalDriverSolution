using System;
using System.Threading;
using Fw21;
using Serilog;
//  библиотека с EcrCtrl

// для AppLogger

namespace DriverWindowsService
{
    public class EcrDriverInitializer : IDisposable
    {
        #region

        public bool IsInitialized { get; private set; }
        public int ComPort { get; }
        public int BaudRate { get; }

        #endregion

        #region

        private EcrCtrl _ecrCtrl;
        private PilotLogger _pilotLogger = new PilotLogger();
        #endregion

        public EcrDriverInitializer(int comPort, int baudRate)
        {
            ComPort = comPort;
            BaudRate = baudRate;
        }

        public void Dispose()
        {
            try
            {
                (_ecrCtrl as IDisposable)?.Dispose();
                Log.Information("ECR освобождён и ресурсы закрыты");
            }
            catch (Exception ex)
            {
                Log.Information("Ошибка при Dispose ECR", ex);
            }
            finally
            {
                _ecrCtrl = null;
                IsInitialized = false;
            }
        }

        public void Init()
        {
            if (IsInitialized)
                throw new InvalidOperationException("ECR driver уже инициализирован");

            try
            {
                Log.Information($"Запуск инициализации ECR на COM{ComPort}, {BaudRate} бод");

                _ecrCtrl = new EcrCtrl();
                _ecrCtrl.Init(ComPort, BaudRate);
                _ecrCtrl.OnSessionCompleted += data =>
                {
                    if (Thread.CurrentThread.Name != "OFD")
                        data.Flush(_pilotLogger);
                };
                IsInitialized = true;
                Log.Information("ECR успешно инициализирован");
            }
            catch (Exception ex)
            {
                Log.Error("Ошибка инициализации ECR", ex);
                Dispose();
                throw;
            }
        }

        public EcrCtrl GetEcr()
        {
            if (!IsInitialized)
                throw new InvalidOperationException("ECR не инициализирован");
            return _ecrCtrl;
        }
    }
}