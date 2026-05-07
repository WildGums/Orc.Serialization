namespace Orc.Serialization.Json.Tests;

using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
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

    private abstract class AbstractAnimal
    {
    }

    private sealed class Dog : AbstractAnimal
    {
        public string? Name { get; set; }
    }

    private sealed class Cat : AbstractAnimal
    {
        public string? Name { get; set; }
        public int Lives { get; set; }
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
        public async Task Serializes_Enum_As_String_By_Default()
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

        [Test]
        public void Serializes_Abstract_Type_With_Default_Type_Info_Converter()
        {
            var serializer = CreateSerializer();
            AbstractAnimal model = new Dog { Name = "Buddy" };

            using var stream = new MemoryStream();
            serializer.Serialize(stream, model);

            var json = Encoding.UTF8.GetString(stream.ToArray());

            Assert.That(json, Does.Contain("\"__type\""));
            Assert.That(json, Does.Contain("\"__object\""));
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

        [Test]
        public void TypeInfoResolverChain_Deserializes_Abstract_Type()
        {
            var settings = new JsonSerializerSettings();
            settings.TypeInfoResolverChain.Add(CreateAnimalPolymorphismResolver());
            var serializer = CreateSerializer(settings);
            var json = "{\"$type\":\"dog\",\"Name\":\"Buddy\"}";

            using var stream = ToStream(json);
            var result = serializer.Deserialize<AbstractAnimal>(stream);

            Assert.That(result, Is.InstanceOf<Dog>());
            Assert.That(((Dog)result!).Name, Is.EqualTo("Buddy"));
        }

        [Test]
        public void Deserializes_Abstract_Type_With_Default_Type_Info_Converter()
        {
            var serializer = CreateSerializer();
            var json = "{\"__type\":\"Orc.Serialization.Json.Tests.JsonSerializerFacts+Dog\",\"__object\":{\"Name\":\"Buddy\"}}";

            using var stream = ToStream(json);
            var result = serializer.Deserialize<AbstractAnimal>(stream);

            Assert.That(result, Is.InstanceOf<Dog>());
            Assert.That(((Dog)result!).Name, Is.EqualTo("Buddy"));
        }
    }

    [TestFixture]
    public class The_Serializer_Binder
    {
        [Test]
        public void Allows_Explicitly_Allowed_Types()
        {
            var binder = new AllowedTypesSerializerBinder([typeof(SampleModel)]);

            Assert.That(binder.IsTypeAllowed(typeof(SampleModel)), Is.True);
        }

        [Test]
        public void Rejects_Types_That_Are_Not_Allowed()
        {
            var binder = new AllowedTypesSerializerBinder([typeof(SampleModel)]);

            Assert.That(binder.IsTypeAllowed(typeof(Dog)), Is.False);
        }

        [Test]
        public void Throws_When_Serializing_Disallowed_Type()
        {
            var settings = new JsonSerializerSettings
            {
                SerializerBinder = new AllowedTypesSerializerBinder([typeof(SampleModel)])
            };
            var serializer = CreateSerializer(settings);

            using var stream = new MemoryStream();

            Assert.Throws<NotSupportedException>(() => serializer.Serialize(stream, new Dog { Name = "Buddy" }));
        }

        [Test]
        public void Throws_When_Deserializing_Disallowed_Target_Type()
        {
            var settings = new JsonSerializerSettings
            {
                SerializerBinder = new AllowedTypesSerializerBinder([typeof(SampleModel)])
            };
            var serializer = CreateSerializer(settings);
            var json = "{\"Name\":\"Buddy\"}";

            using var stream = ToStream(json);

            Assert.Throws<NotSupportedException>(() => serializer.Deserialize(stream, typeof(Dog)));
        }

        [Test]
        public void Throws_When_Deserializing_Disallowed_Runtime_Type_With_Type_Info()
        {
            var settings = new JsonSerializerSettings
            {
                SerializerBinder = new AllowedTypesSerializerBinder([typeof(AbstractAnimal)])
            };
            var serializer = CreateSerializer(settings);
            var json = "{\"__type\":\"Orc.Serialization.Json.Tests.JsonSerializerFacts+Dog\",\"__object\":{\"Name\":\"Buddy\"}}";

            using var stream = ToStream(json);

            Assert.Throws<NotSupportedException>(() => serializer.Deserialize(stream, typeof(AbstractAnimal)));
        }

        [Test]
        public void Allows_When_Deserializing_Allowed_Runtime_Type_With_Type_Info()
        {
            var settings = new JsonSerializerSettings
            {
                SerializerBinder = new AllowedTypesSerializerBinder([typeof(AbstractAnimal), typeof(Cat)])
            };
            var serializer = CreateSerializer(settings);
            var json = "{\"__type\":\"Orc.Serialization.Json.Tests.JsonSerializerFacts+Cat\",\"__object\":{\"Name\":\"Misty\",\"Lives\":9}}";

            using var stream = ToStream(json);
            var result = serializer.Deserialize<AbstractAnimal>(stream);

            Assert.That(result, Is.InstanceOf<Cat>());
            Assert.That(((Cat)result!).Name, Is.EqualTo("Misty"));
            Assert.That(((Cat)result).Lives, Is.EqualTo(9));
        }
    }

    private static IJsonTypeInfoResolver CreateAnimalPolymorphismResolver()
    {
        var resolver = new DefaultJsonTypeInfoResolver();
        resolver.Modifiers.Add(ConfigureAnimalPolymorphism);
        return resolver;
    }

    private static void ConfigureAnimalPolymorphism(JsonTypeInfo jsonTypeInfo)
    {
        if (jsonTypeInfo.Type != typeof(AbstractAnimal))
        {
            return;
        }

        jsonTypeInfo.PolymorphismOptions = new JsonPolymorphismOptions
        {
            TypeDiscriminatorPropertyName = "$type",
            UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FailSerialization,
            IgnoreUnrecognizedTypeDiscriminators = false,
            DerivedTypes =
            {
                new JsonDerivedType(typeof(Dog), "dog")
            }
        };
    }
}
