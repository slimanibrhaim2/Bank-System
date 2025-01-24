using System;

namespace Bank.DTOs
{
    public class TransactionDTO
    {
        public long AccountNumber { get; set; }
        public string TransactionTypeName { get; set; }
        public double Amount { get; set; }
        
        public DateTime TransactionDate { get; set; }
        public long? RecipientAccountNumber { get; set; } // For transfers

    }
}