using OMS.Domain.Common;
using OMS.Domain.Entities;
using OMS.Domain.ValueObjects;

namespace OMS.Test.Domain.Entities
{
    public class OrderTests
    {
        private readonly Order _defaultOrder = Order.Create(Guid.NewGuid(), CreateAddress());

        private static Address CreateAddress(
            string street = "123 Test St",
            string city = "Kauhajoki",
            string province = "61800",
            string country = "FI"
        ) => Address.Create(street, city, province, country);

        [Fact]
        public void Order_Create_ShouldReturnOrderWithTotalAmountIsZeroEuro()
        {
            Assert.Equal(_defaultOrder.TotalAmount, Money.Zero("EUR"));
        }

        [Fact]
        public void Order_Create_ShouldReturnOrderWithStatusIsDraft()
        {
            Assert.Equal(OrderStatus.Draft, _defaultOrder.Status);
        }

        [Fact]
        public void Order_AddItem_ShouldThrowDomainException_WhenQuantityIsLessThanZero()
        {
            var unitPrice = Money.Create(100, "EUR");
            var exp = Assert.Throws<DomainException>(() =>
                _defaultOrder.AddItem(Guid.NewGuid(), "Product A", unitPrice, -1)
            );
            Assert.Equal("Can not AddItem has negative quantity", exp.Message);
        }

        [Fact]
        public void Order_AddItem_ShouldThrowDomainException_WhenOrderStatusIsNotDraft()
        {
            var unitPrice = Money.Create(100, "EUR");

            _defaultOrder.AddItem(Guid.NewGuid(), "Product A", unitPrice, 2);
            _defaultOrder.PlaceOrder();

            var exp = Assert.Throws<DomainException>(() =>
                _defaultOrder.AddItem(Guid.NewGuid(), "Product B", unitPrice, 1)
            );
            Assert.Equal("Can only AddItem with OrderStatus is Draft", exp.Message);
        }

        [Fact]
        public void Order_AddItem_ShouldAddNewItem_AndRecalculateTotal()
        {
            var unitPrice = Money.Create(100, "EUR");

            _defaultOrder.AddItem(Guid.NewGuid(), "Product A", unitPrice, 2);

            Assert.Single(_defaultOrder.Items);
            Assert.Equal(2, _defaultOrder.Items[0].Quantity);
            Assert.Equal(Money.Create(200, "EUR"), _defaultOrder.TotalAmount);
        }

        [Fact]
        public void Order_AddItem_ShouldIncreaseQuantity_WhenProductAlreadyExists()
        {
            var productId = Guid.NewGuid();
            var unitPrice = Money.Create(50, "EUR");

            _defaultOrder.AddItem(productId, "Product A", unitPrice, 1);
            _defaultOrder.AddItem(productId, "Product A", unitPrice, 3);

            Assert.Single(_defaultOrder.Items);
            Assert.Equal(4, _defaultOrder.Items[0].Quantity);
            Assert.Equal(Money.Create(200, "EUR"), _defaultOrder.TotalAmount);
        }

        [Fact]
        public void Order_RemoveItem_ShouldRemoveItem_AndRecalculateTotal()
        {
            var productId = Guid.NewGuid();
            var unitPrice = Money.Create(100, "EUR");

            _defaultOrder.AddItem(productId, "Product A", unitPrice, 2);
            var orderItemId = _defaultOrder.Items[0].Id;

            _defaultOrder.RemoveItem(orderItemId);

            Assert.Empty(_defaultOrder.Items);
            Assert.Equal(Money.Zero("EUR"), _defaultOrder.TotalAmount);
        }

        [Fact]
        public void Order_RemoveItem_ShouldThrowDomainException_WhenStatusIsNotDraft()
        {
            var productId = Guid.NewGuid();
            var unitPrice = Money.Create(100, "EUR");

            _defaultOrder.AddItem(productId, "Product A", unitPrice, 1);
            var orderItemId = _defaultOrder.Items[0].Id;
            _defaultOrder.PlaceOrder();

            Assert.Throws<DomainException>(() => _defaultOrder.RemoveItem(orderItemId));
        }

        [Fact]
        public void Order_RemoveItem_ShouldThrowDomainException_WhenOrderItemNotFound()
        {
            var exp = Assert.Throws<DomainException>(() =>
                _defaultOrder.RemoveItem(Guid.NewGuid())
            );
            Assert.Contains("not found", exp.Message);
        }

        [Fact]
        public void Order_PlaceOrder_ShouldThrowDomainException_WhenNoItems()
        {
            var exp = Assert.Throws<DomainException>(() => _defaultOrder.PlaceOrder());
            Assert.Equal("Can not PlaceOrder without OrderItem", exp.Message);
        }

        [Fact]
        public void Order_PlaceOrder_ShouldSetStatusPlaced_AndUpdatedAt()
        {
            var unitPrice = Money.Create(100, "EUR");
            _defaultOrder.AddItem(Guid.NewGuid(), "Product A", unitPrice, 1);

            _defaultOrder.PlaceOrder();

            Assert.Equal(OrderStatus.Placed, _defaultOrder.Status);
            Assert.NotNull(_defaultOrder.UpdatedAt);
        }

        [Fact]
        public void Order_PlaceOrder_ShouldThrowDomainException_WhenStatusIsNotDraft()
        {
            var unitPrice = Money.Create(100, "EUR");
            _defaultOrder.AddItem(Guid.NewGuid(), "Product A", unitPrice, 1);
            _defaultOrder.PlaceOrder();

            var exp = Assert.Throws<DomainException>(() => _defaultOrder.PlaceOrder());
            Assert.Equal("Can only PlaceOrder with OrderStatus is Draft", exp.Message);
        }

        [Fact]
        public void Order_UpdateShippingAddress_ShouldUpdateAddress_WhenStatusIsDraft()
        {
            var newAddress = CreateAddress("1 New St", "Helsinki", "00100", "FI");

            _defaultOrder.UpdateShippingAddress(newAddress);

            Assert.Equal(newAddress, _defaultOrder.ShippingAddress);
        }

        [Fact]
        public void Order_UpdateShippingAddress_ShouldUpdateAddress_WhenStatusIsPlaced()
        {
            var unitPrice = Money.Create(100, "EUR");
            var newAddress = CreateAddress("2 New St", "Espoo", "02100", "FI");
            _defaultOrder.AddItem(Guid.NewGuid(), "Product A", unitPrice, 1);
            _defaultOrder.PlaceOrder();

            _defaultOrder.UpdateShippingAddress(newAddress);

            Assert.Equal(newAddress, _defaultOrder.ShippingAddress);
        }
    }
}