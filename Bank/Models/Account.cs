using System;
using System.Collections.Generic;

namespace Bank.Models;

public partial class Account
{
    public long AccountId { get; set; }

    public long UserId { get; set; }

    public long AccountNumber { get; set; }

    public double Balance { get; set; }

    public DateTime CreateAt { get; set; }

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

    public virtual ICollection<TransferBetweenAccount> TransferBetweenAccounts { get; set; } = new List<TransferBetweenAccount>();

    public virtual User User { get; set; } = null!;
}
