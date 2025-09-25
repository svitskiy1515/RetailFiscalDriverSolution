using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Contracts
{
    [DataContract]
    public class PackedResult
    {
        [DataMember] public Guid Id { get; set; }
        [DataMember] public bool Success { get; set; }
        [DataMember] public string ErrorMessage { get; set; }
        [DataMember] public byte[] PackedData { get; set; }
    }

}
