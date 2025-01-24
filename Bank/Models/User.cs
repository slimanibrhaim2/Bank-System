using System;
using System.Collections.Generic;

namespace Bank.Models;

public partial class User
{
    public long UserId { get; set; }

    public string UserName { get; set; } = null!;

    public string UserEmail { get; set; } = null!;

    public string Password { get; set; } = null!;

    public DateTime CreateAt { get; set; }

    public string Phone { get; set; } = null!;

    public string? Address { get; set; }

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();
}
