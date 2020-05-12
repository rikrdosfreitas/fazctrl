using FazCtrl.Contract.Interfaces;
using Microsoft.Azure.Cosmos.Table;

namespace FazCtrl.Infrastructure.Azure
{
    public class EventTableServiceEntity :  TableEntity
    {
        public string SourceId { get; set; }
        public string SourceType { get; set; }
        public string Payload { get; set; }
        public string CreationDate { get; set; }
        public string CorrelationId { get; set; }
        public string AssemblyName { get; set; }
        public string Namespace { get; set; }
        public string FullName { get; set; }
        public string TypeName { get; set; }
    }
}