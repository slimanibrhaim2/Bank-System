using System;
using System.Collections.Generic;

namespace Bank.Models;

public partial class Transaction
{
    public long TransactionId { get; set; }

    public long AccountId { get; set; }

    public long TransactionTypeId { get; set; }

    public double Amount { get; set; }

    public DateTime TransactionDate { get; set; }

    public virtual Account Account { get; set; } = null!;

    public virtual TransactionType TransactionType { get; set; } = null!;

    public virtual ICollection<TransferBetweenAccount> TransferBetweenAccounts { get; set; } = new List<TransferBetweenAccount>();
}
