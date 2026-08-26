using System;
using System.Collections.Generic;
using System.Text;
using OMS.Domain.Entities;

namespace OMS.Domain.ValueObjects
{
    public sealed record ShippingZone
    {
        public string Code { get; }
        public decimal RatePerKg { get; }

        private ShippingZone(string code, decimal ratePerKg)
        {
            Code = code;
            RatePerKg = ratePerKg;
        }

        // Static factory
        public static readonly ShippingZone Domestic = new("DOMESTIC", 5);
        public static readonly ShippingZone Regional = new("REGIONAL", 10);
        public static readonly ShippingZone International = new("INTL", 20);
    }
}