namespace FazCtrl.Common.Serializer
{
    public interface ITextSerializer
    {
        string Serialize(object obj);
        object Deserialize(string text);
        T Deserialize<T>(string text);
    }
}
