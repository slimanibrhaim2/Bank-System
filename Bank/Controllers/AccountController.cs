using Bank.Context;
using Bank.DTOs;
using Bank.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bank.Controllers
{
    public class AccountController : Controller
    {
        protected readonly MyDbContext _context;
        protected readonly ILogger<AccountController> _logger;

        public AccountController(MyDbContext context, ILogger<AccountController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var accounts = await _context.Accounts.ToListAsync();
            foreach (var account in accounts)
            {
                account.User = _context.Users.FirstOrDefault(u => u.UserId == account.UserId);
            }
            return View(accounts);
        }

        // GET: Account/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Account/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AccountDTO accountdto)
        {
            if (ModelState.IsValid)
            {
                var user = _context.Users.FirstOrDefault(u => u.UserEmail == accountdto.UserEmail);
                if (user == null)
                {
                    ModelState.AddModelError("UserEmail", "No user found with the provided email.");
                    return View(accountdto);
                }

                Account account = new Account
                {
                    UserId = user.UserId,
                    AccountNumber = accountdto.AccountNumber,
                    CreateAt = DateTime.Now
                };

                _context.Add(account);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(accountdto);
        }

        // GET: Account/Edit/5
        public async Task<IActionResult> Edit(long id)
        {
            var account = await _context.Accounts.FindAsync(id);
            if (account == null)
            {
                return NotFound();
            }
            return View(account);
        }

        // POST: Account/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, Account account)
        {
            if (id != account.AccountId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(account);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Accounts.Any(e => e.AccountId == id))
                    {
                        return NotFound();
                    }
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(account);
        }

        // GET: Account/Details/5
        public async Task<IActionResult> Details(long id)
        {
            var account = await _context.Accounts
                .Include(a => a.User) // Assuming a navigation property exists for User
                .FirstOrDefaultAsync(m => m.AccountId == id);

            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }
        // DELETE: Account/Delete/5
        // DELETE: Account/Delete/5
        public async Task<IActionResult> Delete(long id)
        {
            var account = await _context.Accounts.FindAsync(id);
            if (account == null)
            {
                return NotFound();
            }

            // Load transactions into memory
            var transactionsOfThisAccount = await _context.Transactions
                .Where(t => t.AccountId == id)
                .ToListAsync();

            foreach (var transaction in transactionsOfThisAccount)
            {
                // Load transfer transactions into memory
                var transferTransactions = await _context.TransferBetweenAccounts
                    .Where(tt => tt.TransactionId == transaction.TransactionId)
                    .ToListAsync();

                _context.TransferBetweenAccounts.RemoveRange(transferTransactions);
            }

            _context.Transactions.RemoveRange(transactionsOfThisAccount);

            _context.Accounts.Remove(account);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        
    }
}
