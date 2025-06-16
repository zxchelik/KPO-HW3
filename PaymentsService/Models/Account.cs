using System.ComponentModel.DataAnnotations;
namespace PaymentsService.Models;

public class Account
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = null!;
    public decimal Balance { get; set; }
    [Timestamp]
    public byte[]? RowVersion { get; set; }
}