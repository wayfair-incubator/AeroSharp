using AeroSharp.Serialization;
using FluentAssertions;
using MessagePack;
using NUnit.Framework;
using MessagePackSerializer = AeroSharp.Serialization.MessagePackSerializer;

namespace AeroSharp.UnitTests.Serialization
{
    [TestFixture]
    [Category("Aerospike")]
    public class MessagePackTests
    {
        [MessagePackObject]
        public class TestType
        {
            [Key(0)]
            public string Text { get; set; }

            [Key(1)]
            public int Number { get; set; }
        }

        [MessagePackObject]
        public class TestType2
        {
            [Key(0)]
            public bool BoolVal { get; set; }
        }

        public class TypeWithoutContractAttributes
        {
            public string Text { get; set; }
        }

        [Test]
        public void Deserialize_to_different_data_type_should_throw_SerializationException()
        {
            var data = new TestType { Text = "someText", Number = 2 };
            var serializer = new MessagePackSerializer();
            var serializedData = serializer.Serialize(data);
            serializer.Invoking(t => t.Deserialize<TestType2>(serializedData)).Should().Throw<SerializationException>()
                .WithMessage("Failed to deserialize.");
        }

        [Test]
        public void Data_should_be_serialized_and_deserialized_successfully()
        {
            var data = new TestType { Text = "someText", Number = 2 };
            var serializer = new MessagePackSerializer();
            var serializedData = serializer.Serialize(data);
            var result = serializer.Deserialize<TestType>(serializedData);
            data.Text.Should().BeEquivalentTo(result.Text);
            data.Number.Should().Be(result.Number);
        }

        [Test]
        public void Serializer_With_Compression_Data_should_be_serialized_and_deserialized_successfully()
        {
            var data = new TestType { Text = "someText", Number = 2 };
            var serializer = new MessagePackSerializerWithCompression();
            var serializedData = serializer.Serialize(data);
            var result = serializer.Deserialize<TestType>(serializedData);
            data.Text.Should().BeEquivalentTo(result.Text);
            data.Number.Should().Be(result.Number);
        }

        [Test]
        public void Serializer_With_Compression_Deserialize_to_different_data_type_should_throw_SerializationException()
        {
            var data = new TestType { Text = "someText", Number = 2 };
            var serializer = new MessagePackSerializerWithCompression();
            var serializedData = serializer.Serialize(data);
            serializer.Invoking(t => t.Deserialize<TestType2>(serializedData)).Should().Throw<SerializationException>()
                .WithMessage("Failed to deserialize.");
        }

        [Test]
        public void Serialize_Without_Contract_Attributes_should_throw_SerializationException()
        {
            var data = new TypeWithoutContractAttributes { Text = "someText" };
            var serializer = new MessagePackSerializer();
            serializer.Invoking(t => t.Serialize(data)).Should().Throw<SerializationException>()
                .WithMessage("Failed to serialize. Did you forget to define your data type contract with MessagePack attributes?");
        }

        [Test]
        public void Compressed_Serializer_Serialize_Without_Contract_Attributes_should_throw_SerializationException()
        {
            var data = new TypeWithoutContractAttributes { Text = "someText" };
            var serializer = new MessagePackSerializerWithCompression();
            serializer.Invoking(t => t.Serialize(data)).Should().Throw<SerializationException>()
                .WithMessage("Failed to serialize. Did you forget to define your data type contract with MessagePack attributes?");
        }
    }
}
