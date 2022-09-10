using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MoneyControl.Models;

namespace MoneyControl.Controllers
{
    public class TransactionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TransactionController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Transaction
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Transcations.Include(t => t.category);
            return View(await applicationDbContext.ToListAsync());
        }



        // GET: Transaction/AddOrEdit
        public IActionResult AddOrEdit(int id=0)
        {
            populateCategories();
            if (id == 0) 
            return View(new Transcation());
            else
                return View(_context.Transcations.Find(id));
        }

        // POST: Transaction/AddOrEdit
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit([Bind("TranscationId,categoryId,Amount,Note,Date")] Transcation transcation)
        {
            if (ModelState.IsValid)
            {
                if (transcation.TranscationId == 0)
                    _context.Add(transcation);
                else
                    _context.Update(transcation);
                    await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            populateCategories();
            return View(transcation);
        }


        // POST: Transaction/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Transcations == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Transcations'  is null.");
            }
            var transcation = await _context.Transcations.FindAsync(id);
            if (transcation != null)
            {
                _context.Transcations.Remove(transcation);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [NonAction]
        public void populateCategories()
        {
            var CategoryCollection = _context.categories.ToList();
            category DefaultCategory = new category() { categoryId = 0, Title = "Choosse a Title" };
            CategoryCollection.Add(DefaultCategory);
            ViewBag.Categories = CategoryCollection;
        
        }

        
    }
}
