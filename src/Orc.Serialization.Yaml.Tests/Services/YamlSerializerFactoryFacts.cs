namespace Orc.Serialization.Yaml.Tests;

using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using VerifyNUnit;

public partial class YamlSerializerFactoryFacts
{
    [TestFixture]
    public class The_CreateSerializer_Method
    {
        [Test]
        public void Creates_Serializer_With_Default_Settings()
        {
            var factory = new YamlSerializerFactory();

            var serializer = factory.CreateSerializer();

            Assert.That(serializer, Is.Not.Null);
            Assert.That(serializer, Is.InstanceOf<IYamlSerializer>());
        }

        [Test]
        public void Creates_Serializer_With_Custom_Settings()
        {
            var factory = new YamlSerializerFactory();
            var settings = new YamlSerializerSettings
            {
                IncludeFields = true
            };

            var serializer = factory.CreateSerializer(settings);

            Assert.That(serializer, Is.Not.Null);
            Assert.That(serializer, Is.InstanceOf<IYamlSerializer>());
        }

        [Test, MethodImpl(MethodImplOptions.NoInlining)]
        public async Task Created_Serializer_Produces_Valid_Yaml()
        {
            var factory = new YamlSerializerFactory();
            var serializer = factory.CreateSerializer();
            var data = new { Id = 1, Name = "Factory" };

            using var stream = new MemoryStream();
            serializer.Serialize(stream, data);

            var yaml = Encoding.UTF8.GetString(stream.ToArray());

            await Verifier.Verify(yaml);
        }
    }
}
