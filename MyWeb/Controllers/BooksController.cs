using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using MyWeb.Models;
using MyWeb.Repositories;
using MyWeb.Services;



namespace MyWeb.Controllers
{
    public class BooksController : Controller
    {
        private readonly IBookRepository _bookRepository;
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly IPhotoService _photoService;
        public BooksController(IBookRepository bookRepository, 
            IStringLocalizer<SharedResource> localizer, 
            IPhotoService photoService)
        {
            _bookRepository = bookRepository;
            _localizer = localizer;
            _photoService = photoService;
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
        [HttpPost]
        public async Task<IActionResult> Create(Book book, IFormFile coverImage)
        {
            ModelState.Remove("CoverImageUrl");
            ModelState.Remove("Category");

            if (ModelState.IsValid)
            {
                if (coverImage != null && coverImage.Length > 0)
                {
                    var imageResult = await _photoService.AddPhotoAsync(coverImage);

                    book.CoverImageUrl = imageResult.SecureUrl.ToString();
                }
                else
                {
                    book.CoverImageUrl = "https://example.com/default-book.png";
                }

                await _bookRepository.AddBookAsync(book);
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var book = await _bookRepository.GetBookByIdAsync(id);

            if (book == null)
            {
                return NotFound("Không tìm thấy cuốn sách này!");
            }

            var categories = await _bookRepository.GetCategoriesAsync();
            ViewData["Categories"] = new SelectList(categories, "Id", "Name");

            return View(book);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int id, Book book, IFormFile? coverImage)
        {
            if (id != book.Id)
            {
                return NotFound();
            }
            ModelState.Remove("CoverImageUrl");
            ModelState.Remove("Category");

            if (ModelState.IsValid)
            {
                try
                {
                    if (coverImage != null && coverImage.Length > 0)
                    {
                        var imageResult = await _photoService.AddPhotoAsync(coverImage);

                        book.CoverImageUrl = imageResult.SecureUrl.ToString();
                    }

                    await _bookRepository.UpdateBookAsync(book);

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Đã xảy ra lỗi khi lưu: " + ex.Message);
                }
            }

            var categories = await _bookRepository.GetCategoriesAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");

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