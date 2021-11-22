using System;
using MessagePack;

namespace AeroSharp.Serialization
{
    internal class MessagePackSerializer : ISerializer
    {
        public byte[] Serialize<T>(T data)
        {
            try
            {
                return MessagePack.MessagePackSerializer.Serialize(data);
            }
            catch (MessagePackSerializationException ex)
            {
                if (ex.InnerException is FormatterNotRegisteredException)
                {
                    throw new SerializationException("Failed to serialize. Did you forget to define your data type contract with MessagePack attributes?", ex.InnerException);
                }

                throw new SerializationException("Failed to serialize.", ex.InnerException);
            }
            catch (Exception e)
            {
                throw new SerializationException("Failed to serialize.", e.InnerException);
            }
        }

        public T Deserialize<T>(byte[] serializedData)
        {
            try
            {
                return MessagePack.MessagePackSerializer.Deserialize<T>(serializedData);
            }
            catch (Exception e)
            {
                throw new SerializationException("Failed to deserialize.", e.InnerException);
            }
        }
    }

    internal class MessagePackSerializerWithCompression : ISerializer
    {
        private readonly MessagePackSerializerOptions _options;

        public MessagePackSerializerWithCompression()
        {
            _options = MessagePackSerializerOptions.Standard.WithCompression(MessagePackCompression.Lz4Block);
        }

        public byte[] Serialize<T>(T data)
        {
            try
            {
                return MessagePack.MessagePackSerializer.Serialize(data, _options);
            }
            catch (MessagePackSerializationException ex)
            {
                if (ex.InnerException is FormatterNotRegisteredException)
                {
                    throw new SerializationException("Failed to serialize. Did you forget to define your data type contract with MessagePack attributes?", ex.InnerException);
                }

                throw new SerializationException("Failed to serialize.", ex.InnerException);
            }
            catch (Exception e)
            {
                throw new SerializationException("Failed to serialize.", e.InnerException);
            }
        }

        public T Deserialize<T>(byte[] serializedData)
        {
            try
            {
                return MessagePack.MessagePackSerializer.Deserialize<T>(serializedData, _options);
            }
            catch (Exception e)
            {
                throw new SerializationException("Failed to deserialize.", e.InnerException);
            }
        }
    }
}
