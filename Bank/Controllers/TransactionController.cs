    using Bank.Context;
    using Bank.Models;
    using Bank.DTOs;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Elfie.Serialization;

    namespace Bank.Controllers
    {
        public class TransactionController : Controller
        {
            protected readonly MyDbContext _context;
            protected readonly ILogger<TransactionController> _logger;
            
            public TransactionController(MyDbContext context, ILogger<TransactionController> logger)
            {
                _context = context;
                _logger = logger;
            }

            // GET: Transactions
            public async Task<IActionResult> Index(long acountId)
            {
                var transactions = await _context.Transactions
                    .Include(t => t.Account)
                    .Include(t => t.TransactionType)
                    .Where(t => t.AccountId == acountId) 
                    .ToListAsync();
                Console.WriteLine("\n\n" + transactions.Count);
                return View(transactions);
            }
            
            // GET: Transaction/Create
            public IActionResult Create(long accountId)
            {
                var account = _context.Accounts.Find(accountId);
                var accountNumber = account.AccountNumber;
                TransactionDTO dto = new TransactionDTO();
                dto.AccountNumber = accountNumber;
                return View(dto);
            }
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create(TransactionDTO transactionDto)
{
    // var account1 = _context.Accounts.Find(accountId);
    if (ModelState.IsValid)
    {
        // Validate Account and Transaction Type
        
        var account = await _context.Accounts.FirstOrDefaultAsync(a => a.AccountNumber == transactionDto.AccountNumber);
        Console.WriteLine();
        var transactionType = await _context.TransactionTypes.FirstOrDefaultAsync(t => t.Name == transactionDto.TransactionTypeName);

        if (account == null)
        {
            ModelState.AddModelError("AccountNumber", "Account not found.");
            return View(transactionDto);
        }

        if (transactionType == null)
        {
            ModelState.AddModelError("TransactionTypeName", "Transaction type not found.");
            return View(transactionDto);
        }

        Console.WriteLine($"Transaction Type from DB: '{transactionType.Name}'");
        Console.WriteLine($"Expected: 'Withdraw', Actual: '{transactionType.Name}'");
        Console.WriteLine("Is Transaction Type Withdraw: " + (transactionType.Name == "Withdraw  "));
        Console.WriteLine("Is Account Balance Less Than Amount: " + (account.Balance < transactionDto.Amount));

        // Ensure sufficient balance for Withdraw and Transfer
        if ((transactionType.Name == "Withdraw  " || transactionType.Name == "Transfer  ") && account.Balance < transactionDto.Amount)
        {
            Console.WriteLine($"Insufficient funds: Account Balance = {account.Balance}, Amount = {transactionDto.Amount}");
            ModelState.AddModelError("Amount", "Insufficient balance for this transaction.");
            return View(transactionDto);
        }

        // Handle Transfer
        if (transactionType.Name == "Transfer  ")
        {
            if (!transactionDto.RecipientAccountNumber.HasValue)
            {
                ModelState.AddModelError("RecipientAccountNumber", "Recipient account number is required for transfers.");
                return View(transactionDto);
            }

            var recipientAccount = await _context.Accounts.FirstOrDefaultAsync(a => a.AccountNumber == transactionDto.RecipientAccountNumber.Value);
            if (recipientAccount == null)
            {
                ModelState.AddModelError("RecipientAccountNumber", "Recipient account not found.");
                return View(transactionDto);
            }

            // Deduct from sender and add to recipient
            account.Balance -= transactionDto.Amount;
            recipientAccount.Balance += transactionDto.Amount;

            _context.Update(recipientAccount);
        }
        else if (transactionType.Name == "Withdraw  ")
        {
            // Deduct from sender
            account.Balance -= transactionDto.Amount;
        }
        else if (transactionType.Name == "Deposit   ")
        {
            // Add to sender
            account.Balance += transactionDto.Amount;
        }

        // Update sender account balance
        _context.Update(account);

        // Create Transaction
        var transaction = new Transaction
        {
            AccountId = account.AccountId,
            TransactionTypeId = transactionType.TransactionTypeId,
            Amount = transactionDto.Amount,
            TransactionDate = DateTime.Now
        };

        _context.Add(transaction);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    return View(transactionDto);
}
        // GET: Transaction/Delete/5
            public async Task<IActionResult> Delete(long id)
            {
                var transaction = await _context.Transactions
                    .Include(t => t.Account)
                    .Include(t => t.TransactionType)
                    .FirstOrDefaultAsync(m => m.TransactionId == id);
                if (transaction == null)
                {
                    return NotFound();
                }

                return View(transaction);
            }

            // POST: Transaction/Delete/5
            [HttpPost, ActionName("Delete")]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> DeleteConfirmed(long id)
            {
                var transaction = await _context.Transactions.FindAsync(id);
                if (transaction != null)
                {
                    _context.Transactions.Remove(transaction);
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction(nameof(Index));
            }
        }
    }
