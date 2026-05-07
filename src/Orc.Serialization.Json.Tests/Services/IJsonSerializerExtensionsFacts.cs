namespace Orc.Serialization.Json.Tests;

using System.Collections.Generic;
using NUnit.Framework;

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

        [Test]
        public void Serializes_Complex_Nested_Object_To_Json_String()
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

            Assert.That(json, Is.Not.Null);
            Assert.That(json, Is.Not.Empty);
            Assert.That(json, Does.Contain("\"OrderNumber\":\"ORD-100\""));
            Assert.That(json, Does.Contain("\"ProductName\":\"Widget\""));
            Assert.That(json, Does.Contain("\"ProductName\":\"Gadget\""));
            Assert.That(json, Does.Contain("\"Label\":\"fragile\""));
            Assert.That(json, Does.Contain("\"Label\":\"oversized\""));
        }
    }
}
