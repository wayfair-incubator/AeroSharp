using AeroSharp.Serialization;
using FluentAssertions;
using NUnit.Framework;
using ProtoBuf;

namespace AeroSharp.UnitTests.Serialization
{
    [TestFixture]
    [Category("Aerospike")]
    public class ProtobufSerializerTests
    {
        [ProtoContract]
        public class TestType
        {
            [ProtoMember(1)]
            public string Text { get; set; }

            [ProtoMember(2)]
            public int Number { get; set; }
        }

        [ProtoContract]
        public class TestType2
        {
            [ProtoMember(1)]
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
            var serializer = new ProtobufSerializer();
            var serializedData = serializer.Serialize(data);
            serializer.Invoking(t => t.Deserialize<TestType2>(serializedData)).Should().Throw<SerializationException>()
                .WithMessage("Failed to deserialize.");
        }

        [Test]
        public void Data_should_be_serialized_and_deserialized_successfully()
        {
            var data = new TestType { Text = "someText", Number = 2 };
            var serializer = new ProtobufSerializer();
            var serializedData = serializer.Serialize(data);
            var result = serializer.Deserialize<TestType>(serializedData);
            data.Text.Should().BeEquivalentTo(result.Text);
            data.Number.Should().Be(result.Number);
        }

        [Test]
        public void Serialize_Without_Contract_Attributes_should_throw_SerializationException()
        {
            var data = new TypeWithoutContractAttributes { Text = "someText" };
            var serializer = new ProtobufSerializer();
            serializer.Invoking(t => t.Serialize(data)).Should().Throw<SerializationException>()
                .WithMessage("Failed to serialize. Did you forget to define your data type contract with Protobuf attributes?");
        }
    }
}
