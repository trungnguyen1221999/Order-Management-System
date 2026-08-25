using OMS.Domain.ValueObjects;

namespace OMS.Test.Domain.ValueObjects
{
    public class EmailTests
    {
        [Fact]
        public void Email_Create_ValidEmail_ReturnsEmailObject()
        {
            // Arrange
            var validEmail = "abc@gmail.com";

            // Act & Assert
            var email = Email.Create(validEmail);
            Assert.Equal(validEmail, email.Value);
        }

        [Fact]
        public void Email_Create_InvalidEmail_ThrowsArgumentException()
        {
            // Arrange
            var invalidEmail = "invalid-email";

            // Act
            var ex = Assert.Throws<ArgumentException>(() => Email.Create(invalidEmail));

            // Assert
            Assert.Contains("Invalid email format.", ex.Message);
        }
    }
}
