namespace Orc.Serialization.Yaml.Tests;

using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using NUnit.Framework;
using VerifyNUnit;

public partial class YamlSerializerFacts
{
    [TestFixture]
    public class The_SerializeToString_Method
    {
        [Test]
        public void Serializes_Simple_Object_To_Yaml_String()
        {
            var serializer = CreateSerializer();
            var model = new SampleModel { Name = "Test", Value = 42, Status = Status.Active };

            var yaml = serializer.SerializeToString(model);

            Assert.That(yaml, Is.Not.Null);
            Assert.That(yaml, Is.Not.Empty);
            Assert.That(yaml, Does.Contain("name: Test"));
            Assert.That(yaml, Does.Contain("value: 42"));
        }

        [Test, MethodImpl(MethodImplOptions.NoInlining)]
        public async Task Serializes_Complex_Nested_Object_To_Yaml_String()
        {
            var serializer = CreateSerializer();
            var order = new OrderModel
            {
                OrderNumber = "ORD-100",
                OrderStatus = Status.Active,
                Items = new List<OrderItemModel>
                {
                    new OrderItemModel
                    {
                        ProductName = "Widget",
                        Quantity = 3,
                        Tags = new List<TagModel>
                        {
                            new TagModel { Label = "fragile", Priority = 1 },
                            new TagModel { Label = "express", Priority = 2 }
                        }
                    },
                    new OrderItemModel
                    {
                        ProductName = "Gadget",
                        Quantity = 1,
                        Tags = new List<TagModel>
                        {
                            new TagModel { Label = "oversized", Priority = 3 }
                        }
                    }
                }
            };

            var yaml = serializer.SerializeToString(order);

            await Verifier.Verify(yaml);
        }
    }

    [TestFixture]
    public class The_DeserializeFromString_Method
    {
        [Test]
        public void Deserializes_Simple_Object_From_Yaml_String()
        {
            var serializer = CreateSerializer();
            var yaml = "name: Hello\nvalue: 99\nstatus: Active\n";

            var result = serializer.DeserializeFromString<SampleModel>(yaml);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Name, Is.EqualTo("Hello"));
            Assert.That(result.Value, Is.EqualTo(99));
        }

        [Test]
        public void Deserializes_Complex_Nested_Object_From_Yaml_String()
        {
            var serializer = CreateSerializer();
            var yaml = string.Join("\n", new[]
            {
                "orderNumber: ORD-200",
                "orderStatus: Active",
                "items:",
                "- productName: Alpha",
                "  quantity: 5",
                "  tags:",
                "  - label: cold",
                "    priority: 1",
                "  - label: perishable",
                "    priority: 2",
                "- productName: Beta",
                "  quantity: 2",
                "  tags:",
                "  - label: bulk",
                "    priority: 4",
                string.Empty
            });

            var result = serializer.DeserializeFromString<OrderModel>(yaml);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.OrderNumber, Is.EqualTo("ORD-200"));
            Assert.That(result.Items, Has.Count.EqualTo(2));

            var firstItem = result.Items![0];
            Assert.That(firstItem.ProductName, Is.EqualTo("Alpha"));
            Assert.That(firstItem.Quantity, Is.EqualTo(5));
            Assert.That(firstItem.Tags, Has.Count.EqualTo(2));
            Assert.That(firstItem.Tags![0].Label, Is.EqualTo("cold"));
            Assert.That(firstItem.Tags[1].Label, Is.EqualTo("perishable"));

            var secondItem = result.Items[1];
            Assert.That(secondItem.ProductName, Is.EqualTo("Beta"));
            Assert.That(secondItem.Tags, Has.Count.EqualTo(1));
            Assert.That(secondItem.Tags![0].Priority, Is.EqualTo(4));
        }
    }
}
