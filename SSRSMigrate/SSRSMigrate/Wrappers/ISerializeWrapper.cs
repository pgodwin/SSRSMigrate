
namespace SSRSMigrate.Wrappers
{
    public interface ISerializeWrapper
    {
        T DeserializeObject<T>(string value);
        string SerializeObject(object value);
    }
}
