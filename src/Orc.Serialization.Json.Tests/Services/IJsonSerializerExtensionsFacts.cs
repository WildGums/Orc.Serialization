namespace Orc.Serialization.Json.Tests;

using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using NUnit.Framework;
using VerifyNUnit;

public partial class JsonSerializerFacts
{
    [TestFixture]
    public class The_SerializeToString_Method
    {
        [Test]
        public void Serializes_Simple_Object_To_Json_String()
        {
            var serializer = CreateSerializer();
            var model = new SampleModel { Name = "Test", Value = 42, Status = Status.Active };

            var json = serializer.SerializeToString(model);

            Assert.That(json, Is.Not.Null);
            Assert.That(json, Is.Not.Empty);
            Assert.That(json, Does.Contain("\"Name\":\"Test\""));
            Assert.That(json, Does.Contain("\"Value\":42"));
        }

        [Test, MethodImpl(MethodImplOptions.NoInlining)]
        public async Task Serializes_Complex_Nested_Object_To_Json_String()
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

            var json = serializer.SerializeToString(order);

            await Verifier.Verify(json);
        }
    }

    [TestFixture]
    public class The_DeserializeFromString_Method
    {
        [Test]
        public void Deserializes_Simple_Object_From_Json_String()
        {
            var serializer = CreateSerializer();
            var json = "{\"Name\":\"Hello\",\"Value\":99,\"Status\":0}";

            var result = serializer.DeserializeFromString<SampleModel>(json);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Name, Is.EqualTo("Hello"));
            Assert.That(result.Value, Is.EqualTo(99));
        }

        [Test]
        public void Deserializes_Complex_Nested_Object_From_Json_String()
        {
            var serializer = CreateSerializer();
            var json = """
                {
                    "OrderNumber": "ORD-200",
                    "OrderStatus": 0,
                    "Items": [
                        {
                            "ProductName": "Alpha",
                            "Quantity": 5,
                            "Tags": [
                                { "Label": "cold", "Priority": 1 },
                                { "Label": "perishable", "Priority": 2 }
                            ]
                        },
                        {
                            "ProductName": "Beta",
                            "Quantity": 2,
                            "Tags": [
                                { "Label": "bulk", "Priority": 4 }
                            ]
                        }
                    ]
                }
                """;

            var result = serializer.DeserializeFromString<OrderModel>(json);

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
