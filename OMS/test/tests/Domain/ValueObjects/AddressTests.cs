using System;
using System.Collections.Generic;
using System.Text;
using OMS.Domain.Common;
using OMS.Domain.ValueObjects;

namespace OMS.Test.Domain.ValueObjects
{
    public class AddressTests
    {
        [Fact]
        public void Address_Create_ShouldThrowDomainException_WhenStreetIsEmpty()
        {
            // Arrange
            string street = "";
            string city = "Helsinki";
            string province = "Uusimaa";
            string country = "FI";
            // Act & Assert
            var exception = Assert.Throws<DomainException>(() =>
                Address.Create(street, city, province, country)
            );
            Assert.Equal("Street can not be empty", exception.Message);
        }

        [Fact]
        public void Address_Create_ShouldThrowDomainException_WhenCityIsEmpty()
        {
            // Arrange
            string street = "Mannerheimintie 1";
            string city = "";
            string province = "Uusimaa";
            string country = "FI";
            // Act & Assert
            var exception = Assert.Throws<DomainException>(() =>
                Address.Create(street, city, province, country)
            );
            Assert.Equal("City can not be empty", exception.Message);
        }

        [Fact]
        public void Address_Create_ShouldThrowDomainException_WhenCountryIsNotValid()
        {
            // Arrange
            string street = "Mannerheimintie 1";
            string city = "Helsinki";
            string province = "Uusimaa";
            string country = "F";
            // Act & Assert
            var exception = Assert.Throws<DomainException>(() =>
                Address.Create(street, city, province, country)
            );
            Assert.Contains("Country code must have 2 letters", exception.Message);
        }

        [Fact]
        public void Address_Create_ShouldReturnAddress_WhenValidInput()
        {
            // Arrange
            string street = "Mannerheimintie 1";
            string city = "Helsinki";
            string province = "Uusimaa";
            string country = "FI";
            string postalCode = "00100";
            // Act
            var address = Address.Create(street, city, province, country, postalCode);
            // Assert
            Assert.Equal(street, address.Street);
            Assert.Equal(city, address.City);
            Assert.Equal(province, address.Province);
            Assert.Equal(country, address.Country);
            Assert.Equal(postalCode, address.PostalCode);
        }
    }
}