using System;
using System.Collections.Generic;
using System.Text;
using OMS.Domain.Common;
using OMS.Domain.ValueObjects;

namespace OMS.Test.Domain.ValueObjects
{
    public class MoneyTests
    {
        [Fact]
        public void Money_Create_WithNegativeValue_ShouldThrowDomainException()
        {
            // Arrange
            decimal negativeAmount = -100.00m;
            string currency = "USD";
            // Act & Assert
            Assert.Throws<DomainException>(() => Money.Create(negativeAmount, currency));
        }

        [Fact]
        public void Money_WithSameValue_ShouldBeEqual()
        {
            // Arrange
            var money1 = Money.Create(100.00m, "USD");
            var money2 = Money.Create(100.00m, "USD");
            // Act & Assert
            Assert.Equal(money1, money2);
        }

        [Fact]
        public void Money_WithDifferentValue_ShouldNotBeEqual()
        {
            // Arrange
            var money1 = Money.Create(100.00m, "USD");
            var money2 = Money.Create(200.00m, "USD");
            // Act & Assert
            Assert.NotEqual(money1, money2);
        }

        [Fact]
        public void Money_WithDifferentCurrency_ShouldNotBeEqual()
        {
            // Arrange
            var money1 = Money.Create(100.00m, "USD");
            var money2 = Money.Create(100.00m, "EUR");
            // Act & Assert
            Assert.NotEqual(money1, money2);
        }

        [Fact]
        public void Money_Add_WithSameCurrency_ShouldReturnCorrectSum()
        {
            // Arrange
            var money1 = Money.Create(100.00m, "USD");
            var money2 = Money.Create(50.00m, "USD");
            // Act
            var result = money1.Add(money2);
            // Assert
            Assert.Equal(Money.Create(150.00m, "USD"), result);
        }

        [Fact]
        public void Money_Add_WithDifferentCurrency_ShouldThrowException()
        {
            // Arrange
            var money1 = Money.Create(100.00m, "USD");
            var money2 = Money.Create(50.00m, "EUR");
            // Act & Assert
            Assert.Throws<DomainException>(() => money1.Add(money2));
        }

        [Fact]
        public void Money_Multiply_ShouldReturnCorrectResult()
        {
            // Arrange
            var money = Money.Create(100.00m, "USD");
            // Act
            var result = money.Multiply(2);
            // Assert
            Assert.Equal(Money.Create(200.00m, "USD"), result);
        }
    }
}