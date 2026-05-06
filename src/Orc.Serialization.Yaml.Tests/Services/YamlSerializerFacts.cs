namespace Orc.Serialization.Yaml.Tests;

using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using VerifyNUnit;

public partial class YamlSerializerFacts
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

    private static IYamlSerializer CreateSerializer(YamlSerializerSettings? settings = null)
    {
        return new YamlSerializer(settings ?? new YamlSerializerSettings());
    }

    private static Stream ToStream(string yaml)
    {
        return new MemoryStream(Encoding.UTF8.GetBytes(yaml));
    }

    [TestFixture]
    public class The_Serialize_Method
    {
        [Test, MethodImpl(MethodImplOptions.NoInlining)]
        public async Task Serializes_Simple_Object_To_Yaml()
        {
            var serializer = CreateSerializer();
            var model = new SampleModel { Name = "Test", Value = 42, Status = Status.Active };

            using var stream = new MemoryStream();
            serializer.Serialize(stream, model);

            stream.Position = 0;
            var yaml = Encoding.UTF8.GetString(stream.ToArray());

            await Verifier.Verify(yaml);
        }

        [Test]
        public void Serializes_Properties_Using_CamelCase_Keys()
        {
            var serializer = CreateSerializer();
            var model = new SampleModel { Name = "CamelTest", Value = 0, Status = Status.Active };

            using var stream = new MemoryStream();
            serializer.Serialize(stream, model);

            var yaml = Encoding.UTF8.GetString(stream.ToArray());

            Assert.That(yaml, Does.Contain("name:"));
            Assert.That(yaml, Does.Contain("value:"));
            Assert.That(yaml, Does.Contain("status:"));
        }
    }

    [TestFixture]
    public class The_Deserialize_Method
    {
        [Test]
        public void Deserializes_Simple_Object_From_Yaml()
        {
            var serializer = CreateSerializer();
            var yaml = "name: Hello\nvalue: 99\nstatus: Active\n";

            using var stream = ToStream(yaml);
            var result = serializer.Deserialize<SampleModel>(stream);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Name, Is.EqualTo("Hello"));
            Assert.That(result.Value, Is.EqualTo(99));
        }

        [Test]
        public void Deserializes_Enum_By_Name()
        {
            var serializer = CreateSerializer();
            var yaml = "name: Test\nvalue: 0\nstatus: Pending\n";

            using var stream = ToStream(yaml);
            var result = serializer.Deserialize<SampleModel>(stream);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Status, Is.EqualTo(Status.Pending));
        }

        [Test]
        public void Roundtrip_Serialization()
        {
            var serializer = CreateSerializer();
            var original = new SampleModel { Name = "Roundtrip", Value = 7, Status = Status.Inactive };

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
