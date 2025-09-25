using System;
using System.ServiceModel;

namespace Shared.Contracts
{
    [ServiceContract]
    public interface IRemoteDriverService
    {
        [OperationContract] Guid EnqueueCommand(PosCommand command);
        [OperationContract] PosCommandResult GetCommandResult(Guid commandId);
        [OperationContract] decimal GetTurnover(string posId);
    }
}
