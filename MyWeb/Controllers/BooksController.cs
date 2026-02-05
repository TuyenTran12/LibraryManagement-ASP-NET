using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyWeb.Data;
using MyWeb.Models;
using MyWeb.Repositories;
using System.Threading.Tasks;
using static System.Reflection.Metadata.BlobBuilder;

namespace MyWeb.Controllers
{
    public class BooksController : Controller
    {
        private readonly IBookRepository _bookRepository;
        public BooksController(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var books = await _bookRepository.GetAllBooksAsync();
            return View(books);
        }
        public async Task<IActionResult> Create()
        {
            var categories = await _bookRepository.GetCategoriesAsync();
            ViewData["Categories"] = new SelectList(categories, "Id", "Name");
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Book book)
        {
            if (ModelState.IsValid)
            {
                await _bookRepository.AddBookAsync(book);
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }
        public async Task<IActionResult> Update(int id)
        {
            var book = await _bookRepository.GetBookByIdAsync(id);
            var categories = await _bookRepository.GetCategoriesAsync();
            ViewData["Categories"] = new SelectList(categories, "Id", "Name");
            return View(book);
        }
        [HttpPost]
        public async Task<IActionResult> Update(Book book)
        {
            if (ModelState.IsValid)
            {
                await _bookRepository.UpdateBookAsync(book);
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }
        public async Task<IActionResult> Delete(int id)
        {
            var book = await _bookRepository.GetBookByIdAsync(id);
            return View(book);
        }
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _bookRepository.DeleteBookAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}