using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using OrdersService.Controllers;
using OrdersService.Data;
using OrdersService.Models;
using Xunit;

namespace OrdersService.Tests
{
    public class OrdersControllerTests
    {
        private readonly DbContextOptions<OrdersDbContext> _options;

        public OrdersControllerTests()
        {
            _options = new DbContextOptionsBuilder<OrdersDbContext>()
                .UseInMemoryDatabase(databaseName: "OrdersTestDb_" + Guid.NewGuid().ToString())
                .Options;
        }

        

        [Fact]
        public async Task List_ShouldReturnOrders_ForSpecificUser()
        {
            var userId = "user123";
            var otherUserId = "user456";
            
            using (var context = new OrdersDbContext(_options))
            {
                context.Orders.AddRange(
                    new Order { Id = Guid.NewGuid(), UserId = userId, Amount = 100m, Description = "Order 1" },
                    new Order { Id = Guid.NewGuid(), UserId = userId, Amount = 200m, Description = "Order 2" },
                    new Order { Id = Guid.NewGuid(), UserId = otherUserId, Amount = 300m, Description = "Other user order" }
                );
                await context.SaveChangesAsync();
            }
            
            using var contextForTest = new OrdersDbContext(_options);
            var controller = new OrdersController(contextForTest);

            var result = await controller.List(userId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var orders = Assert.IsAssignableFrom<IEnumerable<Order>>(okResult.Value);
            Assert.Equal(2, orders.Count());
            Assert.All(orders, order => Assert.Equal(userId, order.UserId));
        }

        [Fact]
        public async Task Get_ShouldReturnOrder_WhenOrderExists()
        {
            var orderId = Guid.NewGuid();
            var userId = "user123";
            
            using (var context = new OrdersDbContext(_options))
            {
                context.Orders.Add(new Order 
                { 
                    Id = orderId, 
                    UserId = userId, 
                    Amount = 100m, 
                    Description = "Test order" 
                });
                await context.SaveChangesAsync();
            }
            
            using var contextForTest = new OrdersDbContext(_options);
            var controller = new OrdersController(contextForTest);

            var result = await controller.Get(orderId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var order = Assert.IsType<Order>(okResult.Value);
            Assert.Equal(orderId, order.Id);
            Assert.Equal(userId, order.UserId);
        }

        [Fact]
        public async Task Get_ShouldReturnNotFound_WhenOrderDoesNotExist()
        {
            using var context = new OrdersDbContext(_options);
            var controller = new OrdersController(context);
            var nonExistentOrderId = Guid.NewGuid();

            var result = await controller.Get(nonExistentOrderId);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task List_ShouldReturnEmptyList_WhenNoOrdersForUser()
        {
            var userId = "userWithNoOrders";
            
            using var context = new OrdersDbContext(_options);
            var controller = new OrdersController(context);

            var result = await controller.List(userId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var orders = Assert.IsAssignableFrom<IEnumerable<Order>>(okResult.Value);
            Assert.Empty(orders);
        }
    }
} 