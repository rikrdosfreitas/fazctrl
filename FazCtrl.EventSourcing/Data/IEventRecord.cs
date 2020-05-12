namespace FazCtrl.EventSourcing.Data
{
    public interface IEventRecord
    {
        string SourceId { get; set; }
        string SourceType { get; }
        string Payload { get; }
        string CreationDate { get; }
        string CorrelationId { get; }

        // Standard metadata
        string AssemblyName { get; }
        string Namespace { get; }
        string FullName { get; }
        string TypeName { get; }
    }
}