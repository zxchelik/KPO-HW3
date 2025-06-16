namespace PaymentsService.Models;
public record PaymentRequest(Guid OrderId, string UserId, decimal Amount);