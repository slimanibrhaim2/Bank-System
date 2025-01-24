namespace Bank.DTOs;

public class AccountDTO
{
    public long AccountNumber { get; set; }
    
    public decimal Balance { get; set; }
    public string UserEmail { get; set; } = null!;
}