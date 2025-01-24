using System;
using System.Collections.Generic;

namespace Bank.Models;

public partial class TransactionType
{
    public long TransactionTypeId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
