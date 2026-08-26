using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using OMS.Domain.Entities;
using OMS.Domain.Services;
using OMS.Domain.ValueObjects;

namespace OMS.Test.Domain.Services
{
    public class OrderPricingServiceTests
    {
        private readonly OrderPricingService _orderPricingService = new OrderPricingService();

        private static void SetProductWeightForAllItems(Order order, decimal weightKg)
        {
            foreach (var item in order.Items)
            {
                var product = Product.Create(
                    name: "Test Product",
                    description: "For pricing test",
                    price: Money.Create(100m, "EUR"),
                    weightKg: weightKg,
                    stockQuantity: 10
                );

                var productProp = typeof(OrderItem).GetProperty(
                    "Product",
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
                );

                productProp!.SetValue(item, product);
            }
        }

        private static Address CreateAddress(
            string street = "123 Test St",
            string city = "Kauhajoki",
            string province = "61800",
            string country = "FI"
        ) => Address.Create(street, city, province, country);

        private static List<OrderItem> CreateOrderItemsList()
        {
            var temp = Order.Create(Guid.NewGuid(), CreateAddress());
            temp.AddItem(Guid.NewGuid(), "Product D", Money.Create(100, "EUR"), 5);
            return temp.Items.ToList();
        }

        private readonly Order _orderWithItems = Order.Create(
            Guid.NewGuid(),
            CreateAddress(),
            CreateOrderItemsList()
        );

        [Fact]
        public void GoldCustomer_ShouldGet10PercentDiscount()
        {
            var customerTier = CustomerTier.Gold; //10%
            var shippingZone = ShippingZone.Domestic; // 5
            SetProductWeightForAllItems(_orderWithItems, 2m);

            //Act
            var finalPrice = _orderPricingService.CalculateFinalPrice(
                _orderWithItems,
                customerTier,
                shippingZone
            );
            //Assert
            Assert.Equal(500, finalPrice.Amount);
        }
    }
}