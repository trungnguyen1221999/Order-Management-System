using System;
using System.Collections.Generic;
using System.Text;
using OMS.Domain.Common;

namespace OMS.Domain.ValueObjects
{
    public sealed record Money(decimal Amount, string Currency)
    {
        public static Money Create(decimal amount, string currency)
        {
            if (amount < 0)
                throw new DomainException("Money cannot be negative");

            if (string.IsNullOrWhiteSpace(currency) || currency.Trim().Length != 3)
                throw new DomainException(
                    "Currency code must have 3 letters base on ISO 4217 standard"
                );
            return new Money(amount, currency.Trim().ToUpperInvariant());
        }

        public Money Add(Money other)
        {
            EnsureSameCurrency(other);
            return this with { Amount = Amount + other.Amount };
        }

        public Money Subtract(Money other)
        {
            EnsureSameCurrency(other);
            if (Amount < other.Amount)
                throw new DomainException("Result Amount after Subtract can not be negative");

            return this with
            {
                Amount = Amount - other.Amount,
            };
        }

        public Money Multiply(decimal factor)
        {
            if (factor < 0)
                throw new DomainException("Factor can not be negative");

            return this with
            {
                Amount = Math.Round(Amount * factor, 2),
            };
        }

        private void EnsureSameCurrency(Money other)
        {
            if (other.Currency != Currency)
                throw new DomainException($"{Currency} and {other.Currency} are not the same");
        }

        // helpers

        public static Money Zero(string currency) => Create(0, currency);

        public static Money FromUSD(decimal amount) => Create(amount, "USD");

        public static Money FromEUR(decimal amount) => Create(amount, "EUR");

        public override string ToString() => $"{Amount:N2} {Currency}";
    }
}