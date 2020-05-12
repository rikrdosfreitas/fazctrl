using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace FazCtrl.Contract.Interfaces
{
    public class EventData : IEventData
    {
        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
#if (DEBUG)
            Formatting = Formatting.Indented,
#endif
            TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Full,
            TypeNameHandling = TypeNameHandling.All,
            ContractResolver = new SisoJsonDefaultContractResolver(),

        };
        
        [JsonConstructor]
        private EventData()
        {
            
        }

        public EventData(IDomainEvent @event)
        {
            SourceId = @event.SourceId.ToString();
            Payload = Serialize(@event);
            AssemblyName = Path.GetFileNameWithoutExtension(@event.GetType().Assembly.ManifestModule.FullyQualifiedName);
            Namespace = @event.GetType().Namespace;
            FullName = @event.GetType().FullName;
            TypeName = @event.GetType().Name;
        }

       public string SourceId { get; private set; }

        public string SourceType { get; private set; }

        public string Payload { get; private set; }

        public string AssemblyName { get; private set; }

        public string Namespace { get; private set; }

        public string FullName { get; private set; }

        public string TypeName { get; private set; }

        public IDomainEvent Event => Deserialize();

        private IDomainEvent Deserialize()
        {
            var deserializeObject = JsonConvert.DeserializeObject(Payload, Settings);

            return deserializeObject as IDomainEvent;
        }

        private string Serialize(object value)
        {
            return JsonConvert.SerializeObject(value, Settings);
        }

    }

    public class SisoJsonDefaultContractResolver : DefaultContractResolver
    {
        /// <inheritdoc />
        protected override JsonProperty CreateProperty(
            MemberInfo member,
            MemberSerialization memberSerialization)
        {
            //TODO: Maybe cache
            var prop = base.CreateProperty(member, memberSerialization);

            if (!prop.Writable)
            {
                var property = member as PropertyInfo;
                if (property != null)
                {
                    var hasPrivateSetter = property.GetSetMethod(true) != null;
                    prop.Writable = hasPrivateSetter;
                }
            }

            return prop;
        }
    }
}
