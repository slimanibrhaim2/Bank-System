using System;
using System.Collections.Generic;
using Bank.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Bank.Context;

public partial class MyDbContext: IdentityDbContext<User, IdentityRole<long>, long>
{
    public MyDbContext()
    {
    }

    public MyDbContext(DbContextOptions<MyDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    public virtual DbSet<TransactionType> TransactionTypes { get; set; }

    public virtual DbSet<TransferBetweenAccount> TransferBetweenAccounts { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=LAPTOP-4EV3LF36;Database=BankSystem;Trusted_Connection=True;TrustServerCertificate=true");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Account>(entity =>
        {
            entity.Property(e => e.CreateAt).HasColumnType("datetime");

            entity.HasOne(d => d.User).WithMany(p => p.Accounts)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Accounts_User");
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.ToTable("Transaction");

            entity.Property(e => e.TransactionDate).HasColumnType("datetime");

            entity.HasOne(d => d.Account).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Transaction_Accounts");

            entity.HasOne(d => d.TransactionType).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.TransactionTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Transaction_TransactionTypes");
        });

        modelBuilder.Entity<TransactionType>(entity =>
        {
            entity.Property(e => e.TransactionTypeId).ValueGeneratedNever();
            entity.Property(e => e.Name)
                .HasMaxLength(10);

            entity.HasData(
                new List<TransactionType>()
                {
                    new TransactionType()
                    {
                        TransactionTypeId = 1,
                        Name = "Deposit"
                    },
                    new TransactionType()
                    {
                        TransactionTypeId = 2,
                        Name = "Withdraw"
                    },
                    new TransactionType()
                    {
                        TransactionTypeId = 3,
                        Name = "Transfer"
                    }
                });
        });

        modelBuilder.Entity<TransferBetweenAccount>(entity =>
        {
            entity.HasKey(e => e.TransferId).HasName("PK_TransferBetweenAccounts_1");

            entity.Property(e => e.TransferId)
                .HasMaxLength(10)
                .IsFixedLength();

            entity.HasOne(d => d.Recipient).WithMany(p => p.TransferBetweenAccounts)
                .HasForeignKey(d => d.RecipientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TransferBetweenAccounts_Accounts");

            entity.HasOne(d => d.Transaction).WithMany(p => p.TransferBetweenAccounts)
                .HasForeignKey(d => d.TransactionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TransferBetweenAccounts_Transaction");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("User");
            entity.Property(e => e.Address).HasMaxLength(100);
            entity.Property(e => e.CreateAt).HasColumnType("datetime");
            entity.Property(e => e.UserName).HasMaxLength(50);
        });
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
