using Microsoft.AspNetCore.Mvc;
using MyWeb.Models;
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
    }
}