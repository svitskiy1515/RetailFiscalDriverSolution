using System;

namespace Shared.Contracts
{
    public class CommandResult
    {
        public Guid Id { get; }
        public PilotCommand Command { get; }
        public bool Success { get; }
        public string ErrorMessage { get; }

        public CommandResult(Guid id, PilotCommand command, bool success, string errorMessage = null)
        {
            Id = id;
            Command = command;
            Success = success;
            ErrorMessage = errorMessage;
        }
    }
}