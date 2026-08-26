using System;
using System.Collections.Generic;
using System.Text;
using OMS.Domain.Common;
using OMS.Domain.Common.Interfaces;
using OMS.Domain.ValueObjects;

namespace OMS.Domain.Entities
{
    public class Product : Entity, IAuditableEntity
    {
        public string Name { get; private set; } = null!;
        public string Description { get; private set; } = string.Empty;
        public Money Price { get; private set; } = null!;
        public decimal WeightKg { get; private set; }
        public int StockQuantity { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime CreatedAt { get; private set; }

        public DateTime? UpdatedAt { get; private set; }

        private Product()
        { }

        public static Product Create(
            string name,
            string description,
            Money price,
            decimal weightKg,
            int stockQuantity
        )
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Product Name can not be empty");

            if (price.Amount <= 0)
                throw new DomainException("Product Price can not be negative.");

            if (weightKg <= 0)
                throw new DomainException("Product Weight can not be negative");

            if (stockQuantity < 0)
                throw new DomainException("Product Stock can not be negative.");
            return new Product
            {
                Id = Guid.NewGuid(),
                Name = name,
                Description = description,
                Price = price,
                WeightKg = weightKg,
                StockQuantity = stockQuantity,
                CreatedAt = DateTime.UtcNow,
            };
        }
    }
}