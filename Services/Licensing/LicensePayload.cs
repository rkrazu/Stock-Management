using System;
using System.Runtime.Serialization;

namespace Stock_Managemnet.Services.Licensing
{
    [DataContract]
    public sealed class LicensePayload
    {
        [DataMember(Name = "customer")]
        public string Customer { get; set; }

        [DataMember(Name = "machineId")]
        public string MachineId { get; set; }

        [DataMember(Name = "issuedUtc")]
        public string IssuedUtc { get; set; }

        [DataMember(Name = "version")]
        public int Version { get; set; } = 1;
    }
}
