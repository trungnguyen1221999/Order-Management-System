using System;
using System.Collections.Generic;
using System.Text;
using OMS.Domain.Entities;
using OMS.Domain.ValueObjects;

namespace OMS.Domain.Services
{
    public sealed class OrderPricingService
    {
        public Money CalculateFinalPrice(
            Order order,
            CustomerTier customerTier,
            ShippingZone shippingZone
        )
        {
            var subTotal = order.TotalAmount;
            // Apply customer tier discount
            var customerTierDiscount = Money.Create(
                subTotal.Amount * GetDiscountRate(customerTier),
                order.TotalAmount.Currency ?? "EUR"
            );
            // Shipping fee
            var shippingFee = CalculateShippingFee(order, shippingZone);
            return subTotal.Add(shippingFee).Subtract(customerTierDiscount);
        }

        private static decimal GetDiscountRate(CustomerTier tier) =>
            tier switch
            {
                CustomerTier.Silver => 0.05m, // 5%
                CustomerTier.Gold => 0.10m, // 10%
                CustomerTier.Platinum => 0.15m, // 15%
                _ => 0m, // 0%
            };

        private static Money CalculateShippingFee(Order order, ShippingZone zone)
        {
            var totalWeight = order.Items.Sum(i => i.Product.WeightKg * i.Quantity);
            var baseRate = zone.RatePerKg;
            return new Money(totalWeight * baseRate, order.TotalAmount.Currency);
        }
    }
}