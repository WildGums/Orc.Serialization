namespace Orc.Serialization.Yaml.Tests;

using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using VerifyNUnit;

public partial class YamlSerializerFacts
{
    private class TagModel
    {
        public string? Label { get; set; }
        public int Priority { get; set; }
    }

    private class OrderItemModel
    {
        public string? ProductName { get; set; }
        public int Quantity { get; set; }
        public List<TagModel>? Tags { get; init; }
    }

    private class OrderModel
    {
        public string? OrderNumber { get; set; }
        public Status OrderStatus { get; set; }
        public List<OrderItemModel>? Items { get; init; }
    }

    [TestFixture]
    public class The_Serialize_Method_NestedObjects
    {
        [Test, MethodImpl(MethodImplOptions.NoInlining)]
        public async Task Serializes_Three_Level_Nested_Object_To_Yaml()
        {
            var serializer = CreateSerializer();
            var order = new OrderModel
            {
                OrderNumber = "ORD-001",
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

            using var stream = new MemoryStream();
            serializer.Serialize(stream, order);

            stream.Position = 0;
            var yaml = Encoding.UTF8.GetString(stream.ToArray());

            await Verifier.Verify(yaml);
        }
    }

    [TestFixture]
    public class The_Deserialize_Method_NestedObjects
    {
        [Test]
        public void Deserializes_Three_Level_Nested_Object_From_Yaml()
        {
            var serializer = CreateSerializer();
            var yaml = string.Join("\n", new[]
            {
                "orderNumber: ORD-002",
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

            using var stream = ToStream(yaml);
            var result = serializer.Deserialize<OrderModel>(stream);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.OrderNumber, Is.EqualTo("ORD-002"));
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

        [Test]
        public void Roundtrip_Three_Level_Nested_Object()
        {
            var serializer = CreateSerializer();
            var original = new OrderModel
            {
                OrderNumber = "ORD-003",
                OrderStatus = Status.Pending,
                Items = new List<OrderItemModel>
                {
                    new OrderItemModel
                    {
                        ProductName = "Zeta",
                        Quantity = 10,
                        Tags = new List<TagModel>
                        {
                            new TagModel { Label = "new", Priority = 0 },
                            new TagModel { Label = "sale", Priority = 5 }
                        }
                    }
                }
            };

            using var stream = new MemoryStream();
            serializer.Serialize(stream, original);
            stream.Position = 0;

            var result = serializer.Deserialize<OrderModel>(stream);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.OrderNumber, Is.EqualTo(original.OrderNumber));
            Assert.That(result.OrderStatus, Is.EqualTo(original.OrderStatus));
            Assert.That(result.Items, Has.Count.EqualTo(1));

            var item = result.Items![0];
            Assert.That(item.ProductName, Is.EqualTo("Zeta"));
            Assert.That(item.Quantity, Is.EqualTo(10));
            Assert.That(item.Tags, Has.Count.EqualTo(2));
            Assert.That(item.Tags![0].Label, Is.EqualTo("new"));
            Assert.That(item.Tags[0].Priority, Is.EqualTo(0));
            Assert.That(item.Tags[1].Label, Is.EqualTo("sale"));
            Assert.That(item.Tags[1].Priority, Is.EqualTo(5));
        }
    }
}
