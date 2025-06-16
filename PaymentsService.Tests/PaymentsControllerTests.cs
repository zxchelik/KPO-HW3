using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PaymentsService.Controllers;
using PaymentsService.Data;
using PaymentsService.Models;
using Xunit;

namespace PaymentsService.Tests
{
    public class PaymentsControllerTests
    {
        private readonly DbContextOptions<PaymentsDbContext> _options;

        public PaymentsControllerTests()
        {
            _options = new DbContextOptionsBuilder<PaymentsDbContext>()
                .UseInMemoryDatabase(databaseName: "PaymentsTestDb_" + Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public async Task CreateAccount_ShouldCreateAccount_WhenUserDoesNotExist()
        {
            using var context = new PaymentsDbContext(_options);
            var controller = new PaymentsController(context);
            var userId = "newUser123";

            var result = await controller.CreateAccount(userId);

            var createdAtResult = Assert.IsType<CreatedAtActionResult>(result);
            var account = Assert.IsType<Account>(createdAtResult.Value);
            Assert.Equal(userId, account.UserId);
            Assert.Equal(0m, account.Balance);

            var savedAccount = await context.Accounts.FirstOrDefaultAsync(a => a.UserId == userId);
            Assert.NotNull(savedAccount);
        }

        [Fact]
        public async Task CreateAccount_ShouldReturnBadRequest_WhenUserAlreadyExists()
        {
            var userId = "existingUser";
            
            using (var context = new PaymentsDbContext(_options))
            {
                context.Accounts.Add(new Account
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Balance = 0
                });
                await context.SaveChangesAsync();
            }
            
            using var contextForTest = new PaymentsDbContext(_options);
            var controller = new PaymentsController(contextForTest);

            var result = await controller.CreateAccount(userId);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task TopUp_ShouldIncreaseBalance_WhenAccountExists()
        {
            var userId = "userToTopUp";
            var initialBalance = 100m;
            var topUpAmount = 50m;
            var accountId = Guid.NewGuid();
            
            using (var context = new PaymentsDbContext(_options))
            {
                context.Accounts.Add(new Account
                {
                    Id = accountId,
                    UserId = userId,
                    Balance = initialBalance
                });
                await context.SaveChangesAsync();
            }
            
            using var contextForTest = new PaymentsDbContext(_options);
            var controller = new PaymentsController(contextForTest);

            var result = await controller.TopUp(userId, topUpAmount);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var account = Assert.IsType<Account>(okResult.Value);
            Assert.Equal(initialBalance + topUpAmount, account.Balance);
            
            using var contextToVerify = new PaymentsDbContext(_options);
            var updatedAccount = await contextToVerify.Accounts.FindAsync(accountId);
            Assert.Equal(initialBalance + topUpAmount, updatedAccount.Balance);
        }

        [Fact]
        public async Task TopUp_ShouldReturnNotFound_WhenAccountDoesNotExist()
        {
            using var context = new PaymentsDbContext(_options);
            var controller = new PaymentsController(context);
            var nonExistentUserId = "nonExistentUser";
            var amount = 100m;

            var result = await controller.TopUp(nonExistentUserId, amount);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetBalance_ShouldReturnNotFound_WhenAccountDoesNotExist()
        {
            using var context = new PaymentsDbContext(_options);
            var controller = new PaymentsController(context);
            var nonExistentUserId = "nonExistentUser";

            var result = await controller.GetBalance(nonExistentUserId);

            Assert.IsType<NotFoundResult>(result);
        }
        
        private class Anonymous<T>
        {
            public T balance { get; set; }
        }
    }
} 