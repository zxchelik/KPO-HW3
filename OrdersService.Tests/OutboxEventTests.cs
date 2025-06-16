using System;
using OrdersService.Models;
using Xunit;

namespace OrdersService.Tests
{
    public class OutboxEventTests
    {
        [Fact]
        public void OutboxEvent_ShouldBeCreatedWithCorrectProperties()
        {
            var id = Guid.NewGuid();
            var topic = "orders.created";
            var payload = "{\"orderId\":\"123\",\"amount\":100}";
            var createdAt = DateTime.UtcNow;
            var publishedAt = DateTime.UtcNow.AddMinutes(5);
            
            var outboxEvent = new OutboxEvent
            {
                Id = id,
                Topic = topic,
                Payload = payload,
                CreatedAt = createdAt,
                PublishedAt = publishedAt
            };
            
            Assert.Equal(id, outboxEvent.Id);
            Assert.Equal(topic, outboxEvent.Topic);
            Assert.Equal(payload, outboxEvent.Payload);
            Assert.Equal(createdAt, outboxEvent.CreatedAt);
            Assert.Equal(publishedAt, outboxEvent.PublishedAt);
        }
        
        [Fact]
        public void OutboxEvent_ShouldHaveDefaultCreatedAtValue()
        {
            var outboxEvent = new OutboxEvent();
            var now = DateTime.UtcNow;
            
            Assert.True((now - outboxEvent.CreatedAt).TotalSeconds < 5);
        }
        
        [Fact]
        public void OutboxEvent_ShouldAllowNullPublishedAt()
        {
            var outboxEvent = new OutboxEvent
            {
                Id = Guid.NewGuid(),
                Topic = "test.topic",
                Payload = "{}"
            };
            
            Assert.Null(outboxEvent.PublishedAt);
        }
    }
} 