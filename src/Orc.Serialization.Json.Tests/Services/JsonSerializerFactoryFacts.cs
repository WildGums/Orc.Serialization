namespace Orc.Serialization.Json.Tests;

using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using VerifyNUnit;

public partial class JsonSerializerFactoryFacts
{
    [TestFixture]
    public class The_CreateSerializer_Method
    {
        [Test]
        public void Creates_Serializer_With_Default_Settings()
        {
            var factory = new JsonSerializerFactory();

            var serializer = factory.CreateSerializer();

            Assert.That(serializer, Is.Not.Null);
            Assert.That(serializer, Is.InstanceOf<IJsonSerializer>());
        }

        [Test]
        public void Creates_Serializer_With_Custom_Settings()
        {
            var factory = new JsonSerializerFactory();
            var settings = new JsonSerializerSettings
            {
                WriteIndented = false,
                SerializeEnumsAsStrings = true
            };

            var serializer = factory.CreateSerializer(settings);

            Assert.That(serializer, Is.Not.Null);
            Assert.That(serializer, Is.InstanceOf<IJsonSerializer>());
        }

        [Test, MethodImpl(MethodImplOptions.NoInlining)]
        public async Task Created_Serializer_Produces_Valid_Json()
        {
            var factory = new JsonSerializerFactory();
            var serializer = factory.CreateSerializer();
            var data = new { Id = 1, Name = "Factory" };

            using var stream = new MemoryStream();
            serializer.Serialize(stream, data);

            var json = Encoding.UTF8.GetString(stream.ToArray());

            await Verifier.Verify(json);
        }
    }
}
