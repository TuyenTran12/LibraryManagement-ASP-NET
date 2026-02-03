using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyWeb.Data;
using MyWeb.Models;
using static System.Reflection.Metadata.BlobBuilder;

namespace MyWeb.Controllers
{
    public class BooksController : Controller
    {
        public readonly MyAppContext _context;
        public BooksController(MyAppContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var books = await _context.Books
                .Include(c => c.Category)
                .ToListAsync();
            return View(books);
        }
        public IActionResult Create()
        {
            ViewData["Categories"] = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create([Bind("Id, Title, Author, YearPublished, CategoryId")] Book book)
        {
            if(ModelState.IsValid)
            {
                _context.Books.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }
        public async Task<IActionResult> Update(int id)
        {
            ViewData["Categories"] = new SelectList(_context.Categories, "Id", "Name");
            var book = await _context.Books.FirstOrDefaultAsync(b => b.Id == id);
            return View(book);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int id, [Bind("Id, Title, Author, YearPublished, CategoryId")] Book book)
        {
            if(ModelState.IsValid)
            {
                _context.Books.Update(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }return View(book);
        }
        public async Task<IActionResult> Delete(int id)
        {
            var book = await _context.Books.FirstOrDefaultAsync(b => b.Id == id);
            return View(book);
        }
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirm(int id)
        {
            var book = _context.Books.FirstOrDefault(b => b.Id == id);

            if (book == null)
                return NotFound();

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

    }
}
