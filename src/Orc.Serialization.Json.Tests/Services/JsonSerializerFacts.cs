namespace Orc.Serialization.Json.Tests;

using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using VerifyNUnit;

public partial class JsonSerializerFacts
{
    private enum Status
    {
        Active,
        Inactive,
        Pending
    }

    private class SampleModel
    {
        public string? Name { get; set; }
        public int Value { get; set; }
        public Status Status { get; set; }
    }

    private static IJsonSerializer CreateSerializer(JsonSerializerSettings? settings = null)
    {
        return new JsonSerializer(settings ?? new JsonSerializerSettings());
    }

    private static Stream ToStream(string json)
    {
        return new MemoryStream(Encoding.UTF8.GetBytes(json));
    }

    [TestFixture]
    public class The_Serialize_Method
    {
        [Test, MethodImpl(MethodImplOptions.NoInlining)]
        public async Task Serializes_Simple_Object_To_Json()
        {
            var serializer = CreateSerializer();
            var model = new SampleModel { Name = "Test", Value = 42, Status = Status.Active };

            using var stream = new MemoryStream();
            serializer.Serialize(stream, model);

            stream.Position = 0;
            var json = Encoding.UTF8.GetString(stream.ToArray());

            await Verifier.Verify(json);
        }

        [Test, MethodImpl(MethodImplOptions.NoInlining)]
        public async Task Serializes_Enum_As_Number_By_Default()
        {
            var serializer = CreateSerializer();
            var model = new SampleModel { Name = "Test", Value = 1, Status = Status.Inactive };

            using var stream = new MemoryStream();
            serializer.Serialize(stream, model);

            var json = Encoding.UTF8.GetString(stream.ToArray());

            await Verifier.Verify(json);
        }

        [Test, MethodImpl(MethodImplOptions.NoInlining)]
        public async Task Serializes_Enum_As_String_When_SerializeEnumsAsStrings_Is_True()
        {
            var settings = new JsonSerializerSettings { SerializeEnumsAsStrings = true };
            var serializer = CreateSerializer(settings);
            var model = new SampleModel { Name = "Test", Value = 1, Status = Status.Pending };

            using var stream = new MemoryStream();
            serializer.Serialize(stream, model);

            var json = Encoding.UTF8.GetString(stream.ToArray());

            await Verifier.Verify(json);
        }
    }

    [TestFixture]
    public class The_Deserialize_Method
    {
        [Test]
        public void Deserializes_Simple_Object_From_Json()
        {
            var serializer = CreateSerializer();
            var json = "{\"Name\":\"Hello\",\"Value\":99,\"Status\":0}";

            using var stream = ToStream(json);
            var result = serializer.Deserialize<SampleModel>(stream);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Name, Is.EqualTo("Hello"));
            Assert.That(result.Value, Is.EqualTo(99));
        }

        [Test]
        public void Deserializes_Enum_From_Number()
        {
            var serializer = CreateSerializer();
            var json = "{\"Name\":\"Test\",\"Value\":0,\"Status\":2}";

            using var stream = ToStream(json);
            var result = serializer.Deserialize<SampleModel>(stream);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Status, Is.EqualTo(Status.Pending));
        }

        [Test]
        public void Deserializes_Enum_From_String_When_SerializeEnumsAsStrings_Is_True()
        {
            var settings = new JsonSerializerSettings { SerializeEnumsAsStrings = true };
            var serializer = CreateSerializer(settings);
            var json = "{\"Name\":\"Test\",\"Value\":0,\"Status\":\"Inactive\"}";

            using var stream = ToStream(json);
            var result = serializer.Deserialize<SampleModel>(stream);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Status, Is.EqualTo(Status.Inactive));
        }

        [Test]
        public void Roundtrip_Enum_As_String()
        {
            var settings = new JsonSerializerSettings { SerializeEnumsAsStrings = true };
            var serializer = CreateSerializer(settings);
            var original = new SampleModel { Name = "Roundtrip", Value = 7, Status = Status.Active };

            using var stream = new MemoryStream();
            serializer.Serialize(stream, original);
            stream.Position = 0;

            var result = serializer.Deserialize<SampleModel>(stream);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Name, Is.EqualTo(original.Name));
            Assert.That(result.Value, Is.EqualTo(original.Value));
            Assert.That(result.Status, Is.EqualTo(original.Status));
        }
    }
}
