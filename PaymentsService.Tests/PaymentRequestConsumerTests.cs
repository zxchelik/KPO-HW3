using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using PaymentsService.Data;
using PaymentsService.Messaging;
using PaymentsService.Models;
using StackExchange.Redis;
using Xunit;

namespace PaymentsService.Tests
{
    public class PaymentRequestConsumerTests
    {
        private readonly DbContextOptions<PaymentsDbContext> _options;

        public PaymentRequestConsumerTests()
        {
            _options = new DbContextOptionsBuilder<PaymentsDbContext>()
                .UseInMemoryDatabase(databaseName: "PaymentRequestConsumerTestDb_" + Guid.NewGuid().ToString())
                .Options;
        }
        
        
        private IServiceProvider CreateServiceProviderMock(DbContextOptions<PaymentsDbContext> options)
        {
            var services = new ServiceCollection();
            
            services.AddDbContext<PaymentsDbContext>(opt => opt.UseInMemoryDatabase(Guid.NewGuid().ToString()));
            
            var mockServiceProvider = new Mock<IServiceProvider>();
            var mockServiceScope = new Mock<IServiceScope>();
            var mockServiceScopeFactory = new Mock<IServiceScopeFactory>();
            
            mockServiceScope.Setup(x => x.ServiceProvider).Returns(services.BuildServiceProvider());
            mockServiceScopeFactory.Setup(x => x.CreateScope()).Returns(mockServiceScope.Object);
            mockServiceProvider.Setup(x => x.GetService(typeof(IServiceScopeFactory))).Returns(mockServiceScopeFactory.Object);
            
            return mockServiceProvider.Object;
        }
    }
} 