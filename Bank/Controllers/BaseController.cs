using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Bank.Models;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Bank.Context;

namespace Bank.Controllers
{
    public abstract class BaseController<TEntity> : Controller
        where TEntity : class
    {
        protected readonly MyDbContext _context;
        protected readonly ILogger<BaseController<TEntity>> _logger;

        public BaseController(MyDbContext context, ILogger<BaseController<TEntity>> logger)
        {
            _context = context;
            _logger = logger;
        }

       
        // GET: Display details of a specific entity
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entity = await _context.Set<TEntity>().FindAsync(id);
            if (entity == null)
            {
                return NotFound();
            }

            return View(entity);
        }


        // GET: Display the edit form
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entity = await _context.Set<TEntity>().FindAsync(id);
            if (entity == null)
            {
                return NotFound();
            }

            return View(entity);
        }

        // POST: Update an existing entity
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, TEntity entity)
        {
            if (id != GetEntityId(entity))
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(entity);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EntityExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(entity);
        }

        // GET: Display the delete confirmation form
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entity = await _context.Set<TEntity>().FindAsync(id);
            if (entity == null)
            {
                return NotFound();
            }

            return View(entity);
        }

        // POST: Delete an entity
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var entity = await _context.Set<TEntity>().FindAsync(id);
            if (entity != null)
            {
                _context.Set<TEntity>().Remove(entity);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool EntityExists(long id)
        {
            return _context.Set<TEntity>().Any(e => GetEntityId(e) == id);
        }

        // Helper method to get the ID of an entity
        private long GetEntityId(TEntity entity)
        {
            var idProperty = entity.GetType().GetProperties()
                .FirstOrDefault(p => p.Name.EndsWith("Id")); // Assumes primary key ends with "Id"

            if (idProperty == null)
            {
                throw new InvalidOperationException("Entity does not have a primary key property ending with 'Id'.");
            }

            return (long)idProperty.GetValue(entity);
        }
    }
}