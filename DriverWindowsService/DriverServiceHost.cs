using System;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using DriverWindowsService;
using Serilog;

// контракты / модели
// using DriverWindowsService; // если RemoteDriverService в этом пространстве имен

public class DriverServiceHost : IDisposable
{
    #region

    private readonly int _baudRate;
    private readonly int _comPort;
    private Task _backgroundTask;
    private CancellationTokenSource _cts;
    private EcrDriverInitializer _driverInit;
    private ServiceHost _wcfHost;

    #endregion

    public DriverServiceHost(int comPort = 1, int baudRate = 115200)
    {
        _comPort = comPort;
        _baudRate = baudRate;
    }

    public void Dispose()
    {
        try
        {
            Stop();
        }
        catch
        {
        }
    }

    // Called by Topshelf when starting service (or when running as console)
    public bool Start()
    {
        try
        {
            Log.Information("DriverServiceHost starting...");

            // 1) Инициализация драйвера
            _driverInit = new EcrDriverInitializer(_comPort, _baudRate);
            _driverInit.Init();
            Log.Information("ECR initialized");

            // 2) Опционально — поднять WCF-host для RemoteDriverService (если ты хочешь self-host)
            try
            {
                _wcfHost = new ServiceHost(typeof(DriverWindowsService.DriverWindowsService)); // укажи реальный тип
                _wcfHost.Open();
                Log.Information("WCF host opened");
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "WCF host failed to open (maybe not needed). Proceeding.");
            }

            // 3) Запустить фоновые таски (если нужны)
            _cts = new CancellationTokenSource();
            _backgroundTask = Task.Run(() => BackgroundWorker(_cts.Token), _cts.Token);

            Log.Information("DriverServiceHost started");
            return true;
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Error during Start");
            return false;
        }
    }

    // Called by Topshelf when stopping service (or when running as console exit)
    public bool Stop()
    {
        try
        {
            Log.Information("DriverServiceHost stopping...");

            // 1) остановка фоновых задач
            try
            {
                _cts?.Cancel();
                _backgroundTask?.Wait(TimeSpan.FromSeconds(5));
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Error stopping background task");
            }

            // 2) закрыть WCF host
            try
            {
                _wcfHost?.Close();
                _wcfHost = null;
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Error closing WCF host");
            }

            // 3) Dispose драйвера (через IDisposable интерфейс, см. ранее)
            try
            {
                (_driverInit as IDisposable)?.Dispose();
                _driverInit = null;
                Log.Information("ECR disposed");
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Error disposing ECR");
            }

            Log.Information("DriverServiceHost stopped");
            return true;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error during Stop");
            return false;
        }
    }

    private async Task BackgroundWorker(CancellationToken token)
    {
        // пример простого воркера: мониторинг состояния, heartbeat, cleanup
        while (!token.IsCancellationRequested)
            try
            {
                // например, логируем каждые 30 сек
                Log.Debug("Background worker heartbeat");
                await Task.Delay(TimeSpan.FromSeconds(30), token);
            }
            catch (TaskCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Background worker exception");
            }
    }
}

