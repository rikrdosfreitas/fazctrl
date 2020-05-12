using System.Collections.Generic;
using System.Text;

namespace FazCtrl.Contract.Interfaces
{
    public interface IEventData
    {
        string SourceId { get; }
        string SourceType { get; }
        string Payload { get; }


        // Standard metadata
        string AssemblyName { get; }
        string Namespace { get; }
        string FullName { get; }
        string TypeName { get; }

        IDomainEvent Event { get; }
    }
}
