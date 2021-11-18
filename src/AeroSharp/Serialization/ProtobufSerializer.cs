using System;
using System.IO;
using ProtoBuf;

namespace AeroSharp.Serialization
{
    internal class ProtobufSerializer : ISerializer
    {
        public byte[] Serialize<T>(T data)
        {
            try
            {
                using (var ms = new MemoryStream())
                {
                    Serializer.Serialize(ms, data);
                    return ms.ToArray();
                }
            }
            catch (InvalidOperationException e)
            {
                if (e.Message.StartsWith("Type is not expected, and no contract can be inferred"))
                {
                    throw new SerializationException(
                        "Failed to serialize. Did you forget to define your data type contract with Protobuf attributes?", e.InnerException);
                }

                throw new SerializationException("Failed to serialize.", e.InnerException);
            }
            catch (Exception e)
            {
                throw new SerializationException("Failed to serialize.", e.InnerException);
            }
        }

        public T Deserialize<T>(byte[] bytes)
        {
            try
            {
                return Serializer.Deserialize<T>(new MemoryStream(bytes));
            }
            catch (Exception e)
            {
                throw new SerializationException("Failed to deserialize.", e.InnerException);
            }
        }
    }
}
