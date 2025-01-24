using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Bank.Models;

public class User : IdentityUser<long>
{
    public DateTime CreateAt { get; set; }
    public string? Address { get; set; }

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();
}
