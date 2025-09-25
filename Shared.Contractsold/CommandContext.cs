using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Contracts
{
    public class CommandContext
    {
        public Guid Id { get; } = Guid.NewGuid();
        public PilotCommand Command { get; }
        public DateTime EnqueuedAt { get; } = DateTime.UtcNow;

        public CommandContext(PilotCommand cmd)
        {
            Command = cmd;
        }
    }
}
