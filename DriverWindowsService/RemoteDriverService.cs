using System;
using System.Collections.Concurrent;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using Shared.Contracts;

namespace DriverWindowsService
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class RemoteDriverService : IRemoteDriverService, IDisposable
    {
        private readonly ConcurrentQueue<PosCommand> _queue = new ConcurrentQueue<PosCommand>();
        private readonly ConcurrentDictionary<Guid, PosCommandResult> _results = new ConcurrentDictionary<Guid, PosCommandResult>();
        private readonly ConcurrentDictionary<string, decimal> _turnover = new ConcurrentDictionary<string, decimal>();

        private readonly SemaphoreSlim _driverLock = new SemaphoreSlim(1,1);
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        private readonly Task _worker;

        public RemoteDriverService()
        {
            _worker = Task.Run(ProcessQueueAsync);
        }

        public Guid EnqueueCommand(PosCommand command)
        {
            _results[command.CommandId] = new PosCommandResult
            {
                CommandId = command.CommandId,
                PosId = command.PosId,
                IsCompleted = false,
                Error = new Error(0, ErrorCodes.ERR_SUCCESS)
            };
            _queue.Enqueue(command);
            return command.CommandId;
        }

        public PosCommandResult GetCommandResult(Guid commandId)
        {
            return _results.TryGetValue(commandId, out var res) ? res : null;
        }

        public decimal GetTurnover(string posId)
        {
            return _turnover.TryGetValue(posId, out var sum) ? sum : 0m;
        }

        private async Task ProcessQueueAsync()
        {
            while(!_cts.IsCancellationRequested)
            {
                if (_queue.TryDequeue(out var cmd))
                {
                    await _driverLock.WaitAsync();
                    try
                    {
                        // Simulated driver call
                        await Task.Delay(200); // emulate I/O
                        cmd.Command.Run(null);

                        _results[cmd.CommandId] = new PosCommandResult
                        {
                            CommandId = cmd.CommandId,
                            PosId = cmd.PosId,
                            Error = cmd.Command.Error,
                            IsCompleted = true
                        };

                        if (cmd.Command is IFiscalCommand f)
                        {
                            _turnover.AddOrUpdate(cmd.PosId, f.Amount, (_, old) => old + f.Amount);
                        }
                    }
                    catch (Exception ex)
                    {
                        _results[cmd.CommandId] = new PosCommandResult
                        {
                            CommandId = cmd.CommandId,
                            PosId = cmd.PosId,
                            Error = new Error(-1, ErrorCodes.ERR_FAIL){ Description = ex.Message },
                            IsCompleted = true
                        };
                    }
                    finally
                    {
                        _driverLock.Release();
                    }
                }
                else
                {
                    await Task.Delay(25);
                }
            }
        }

        public void Dispose()
        {
            _cts.Cancel();
            try { _worker.Wait(1000); } catch {}
        }
    }
}
