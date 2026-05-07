namespace Orc.Serialization.Yaml.Tests;

using System.Collections.Generic;
using NUnit.Framework;

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

        [Test]
        public void Serializes_Complex_Nested_Object_To_Yaml_String()
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

            Assert.That(yaml, Is.Not.Null);
            Assert.That(yaml, Is.Not.Empty);
            Assert.That(yaml, Does.Contain("orderNumber: ORD-100"));
            Assert.That(yaml, Does.Contain("productName: Widget"));
            Assert.That(yaml, Does.Contain("productName: Gadget"));
            Assert.That(yaml, Does.Contain("label: fragile"));
            Assert.That(yaml, Does.Contain("label: oversized"));
        }
    }
}
