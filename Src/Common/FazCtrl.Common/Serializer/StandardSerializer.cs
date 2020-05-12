using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace FazCtrl.Common.Serializer
{
    public class StandardSerializer : ITextSerializer
    {
        private readonly JsonSerializer _serializer;

        public StandardSerializer()
        {
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
                ContractResolver = new SisoJsonDefaultContractResolver(),
                ReferenceLoopHandling= ReferenceLoopHandling.Ignore,
#if (DEBUG)
            Formatting = Formatting.Indented,
                #endif
            };

            _serializer = JsonSerializer.Create(settings);
        }

        public object Deserialize(string text)
        {
            using (var reader = new StringReader(text))
            using (var jsonReader = new JsonTextReader(reader))
            {
                return _serializer.Deserialize(jsonReader);
            }
        }

        public T Deserialize<T>(string text)
        {
            return (T)Deserialize(text);
        }

        public string Serialize(object obj)
        {
            using (var writer = new StringWriter())
            using (var jsonWriter = new JsonTextWriter(writer))
            {
                _serializer.Serialize(jsonWriter, obj);
                return writer.ToString();
            }
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
