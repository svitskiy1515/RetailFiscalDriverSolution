using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Fw21;
using Shared.Contracts;

namespace DriverWindowsService
{
    public class CommandProcessor
    {
        private readonly ConcurrentQueue<CommandContext> _queue = new();
        private readonly ConcurrentDictionary<Guid, CommandResult> _results = new();
        private readonly CancellationTokenSource _cts = new();
        private readonly EcrCtrl _ecr;

        public CommandProcessor(EcrCtrl ecr)
        {
            _ecr = ecr;
            Task.Run(() => WorkerLoop(_cts.Token));
        }

        public Guid Enqueue(PilotCommand command)
        {
            var ctx = new CommandContext(command);
            _queue.Enqueue(ctx);
            return ctx.Id;
        }

        public CommandResult GetResult(Guid id)
        {
            return _results.TryGetValue(id, out var result) ? result : null;
        }

        private async Task WorkerLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                if (_queue.TryDequeue(out var ctx))
                {
                    try
                    {
                        ctx.Command.Run(_ecr);
                        _results[ctx.Id] = new CommandResult(ctx.Id, ctx.Command, true);
                    }
                    catch (Exception ex)
                    {
                        _results[ctx.Id] = new CommandResult(ctx.Id, ctx.Command, false, ex.Message);
                    }
                }
                else
                {
                    await Task.Delay(50, token); // пауза, если очередь пуста
                }
            }
        }
    }
}