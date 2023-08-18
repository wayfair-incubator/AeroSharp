namespace AeroSharp.Serialization
{
    public interface ISerializer
    {
        byte[] Serialize<T>(T data);

        T Deserialize<T>(byte[] serializedData);
    }
}