using System;
using OrdersService.Models;
using Xunit;

namespace OrdersService.Tests
{
    public class OrderModelTests
    {
        [Fact]
        public void Order_ShouldBeCreatedWithCorrectProperties()
        {
            var id = Guid.NewGuid();
            var userId = "user123";
            var amount = 150.75m;
            var description = "Test order description";
            
            var order = new Order
            {
                Id = id,
                UserId = userId,
                Amount = amount,
                Description = description
            };
            
            Assert.Equal(id, order.Id);
            Assert.Equal(userId, order.UserId);
            Assert.Equal(amount, order.Amount);
            Assert.Equal(description, order.Description);
            Assert.Equal(OrderStatus.NEW, order.Status);
        }
        
        [Fact]
        public void Order_ShouldAllowStatusChange()
        {
            var order = new Order
            {
                Id = Guid.NewGuid(),
                UserId = "user123",
                Amount = 100m,
                Description = "Test order"
            };
            
            order.Status = OrderStatus.FINISHED;
            
            Assert.Equal(OrderStatus.FINISHED, order.Status);
        }
        
        [Theory]
        [InlineData(OrderStatus.NEW)]
        [InlineData(OrderStatus.FINISHED)]
        [InlineData(OrderStatus.CANCELLED)]
        public void Order_ShouldAcceptAllValidStatuses(OrderStatus status)
        {
            var order = new Order();
            
            order.Status = status;
            
            Assert.Equal(status, order.Status);
        }
    }
} 