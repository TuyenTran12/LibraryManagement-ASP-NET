using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using MyWeb.Repositories;

namespace MyWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly IBookRepository _bookRepository;
        private readonly IChapterRepository _chapterRepository;

        public HomeController(IBookRepository bookRepository, IChapterRepository chapterRepository)
        {
            _bookRepository = bookRepository;
            _chapterRepository = chapterRepository;
        }

        public async Task<IActionResult> Index()
        {
            var books = await _bookRepository.GetAllBooksAsync();
            ViewBag.AllCategories = await _bookRepository.GetCategoriesAsync();
            return View(books);
        }
        public async Task<IActionResult> ReadFirst(int bookId)
        {
            var allChapters = await _chapterRepository.GetChaptersByBookIdAsync(bookId);

            var firstChapter = allChapters.OrderBy(c => c.ChapterNumber).FirstOrDefault();

            if (firstChapter != null)
            {
                return RedirectToAction("Read", "Home", new { id = firstChapter.Id });
            }

            TempData["ErrorMessage"] = "Truyện này hiện chưa có chương nào để đọc!";
            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> Read(int id)
        {
            var chapter = await _chapterRepository.GetChapterWithImagesAsync(id);
            if (chapter == null) return NotFound("Không tìm thấy chương truyện này!");

            var allChapters = await _chapterRepository.GetChaptersByBookIdAsync(chapter.BookId);

            var nextChapter = allChapters
                .Where(c => c.ChapterNumber > chapter.ChapterNumber)
                .OrderBy(c => c.ChapterNumber)
                .FirstOrDefault();

            var prevChapter = allChapters
                .Where(c => c.ChapterNumber < chapter.ChapterNumber)
                .OrderByDescending(c => c.ChapterNumber)
                .FirstOrDefault();

            ViewBag.NextChapterId = nextChapter?.Id;
            ViewBag.PrevChapterId = prevChapter?.Id;

            var imageUrls = chapter.Images
                                   .OrderBy(img => img.PageNumber)
                                   .Select(img => img.ImageUrl)
                                   .ToList();

            ViewBag.ChapterTitle = chapter.Title;
            ViewBag.ChapterNumber = chapter.ChapterNumber;
            ViewBag.BookId = chapter.BookId;

            return View(imageUrls);
        }
        [HttpGet]
        public async Task<IActionResult> Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return RedirectToAction("Index");
            }
            var searchResults = await _bookRepository.SearchBooksAsync(query);

            ViewBag.SearchQuery = query;
            ViewBag.AllCategories = await _bookRepository.GetCategoriesAsync();

            return View("Search", searchResults);
        }
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var book = await _bookRepository.GetBookWithDetailsAsync(id);

            if (book == null)
                return NotFound("Không tìm thấy truyện!");

            var similarBooks = await _bookRepository.GetSimilarBooksAsync(book.CategoryId ?? 0, book.Id, 5);

            ViewBag.SimilarBooks = similarBooks;
            ViewBag.AllCategories = await _bookRepository.GetCategoriesAsync();

            return View(book);
        }
        public async Task<IActionResult> SeeAll(string section)
        {
            var books = await _bookRepository.GetAllBooksAsync();


            ViewBag.SectionName = section;
            ViewBag.AllCategories = await _bookRepository.GetCategoriesAsync();
            return View(books);
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ToggleBookmark(int bookId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Json(new { success = false, message = "Bạn cần đăng nhập để đánh dấu truyện!" });
            }
            var result = await _bookRepository.ToggleBookmark(userId, bookId);
            return result;
        }
    }
}