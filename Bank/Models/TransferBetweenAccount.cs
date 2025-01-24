using System;
using System.Collections.Generic;

namespace Bank.Models;

public partial class TransferBetweenAccount
{
    public string TransferId { get; set; } = null!;

    public long TransactionId { get; set; }

    public long RecipientId { get; set; }

    public virtual Account Recipient { get; set; } = null!;

    public virtual Transaction Transaction { get; set; } = null!;
}
