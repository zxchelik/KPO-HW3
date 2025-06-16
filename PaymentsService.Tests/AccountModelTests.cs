using System;
using PaymentsService.Models;
using Xunit;

namespace PaymentsService.Tests
{
    public class AccountModelTests
    {
        [Fact]
        public void Account_ShouldBeCreatedWithCorrectProperties()
        {
            var id = Guid.NewGuid();
            var userId = "user123";
            var balance = 500.25m;
            var rowVersion = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };
            
            var account = new Account
            {
                Id = id,
                UserId = userId,
                Balance = balance,
                RowVersion = rowVersion
            };
            
            Assert.Equal(id, account.Id);
            Assert.Equal(userId, account.UserId);
            Assert.Equal(balance, account.Balance);
            Assert.Equal(rowVersion, account.RowVersion);
        }
        
        [Fact]
        public void Account_ShouldAllowBalanceChange()
        {
            var account = new Account
            {
                Id = Guid.NewGuid(),
                UserId = "user123",
                Balance = 100m
            };
            
            account.Balance += 50m;
            
            Assert.Equal(150m, account.Balance);
        }
        
        [Fact]
        public void Account_ShouldAllowNegativeBalance()
        {
            var account = new Account
            {
                Id = Guid.NewGuid(),
                UserId = "user123",
                Balance = 100m
            };
            
            account.Balance -= 150m;
            
            Assert.Equal(-50m, account.Balance);
        }
    }
} 