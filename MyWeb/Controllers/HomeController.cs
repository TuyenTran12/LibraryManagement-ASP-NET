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
                // Ngay lập tức chuyển hướng sang link chuẩn: Home/Read/ChapterId
                return RedirectToAction("Read", "Home", new { id = firstChapter.Id });
            }

            // Nếu truyện chưa có chương nào, quay lại trang chủ kèm thông báo (tùy chọn)
            TempData["ErrorMessage"] = "Truyện này hiện chưa có chương nào để đọc!";
            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> Read(int id)
        {
            // 1. Lấy thông tin chương hiện tại
            var chapter = await _chapterRepository.GetChapterByIdAsync(id);
            if (chapter == null) return NotFound("Không tìm thấy chương truyện này!");

            // 2. Lấy tất cả chương của bộ truyện này để tìm chương lân cận
            var allChapters = await _chapterRepository.GetChaptersByBookIdAsync(chapter.BookId);

            // Tìm chương có số thứ tự lớn hơn gần nhất (Chương sau)
            var nextChapter = allChapters
                .Where(c => c.ChapterNumber > chapter.ChapterNumber)
                .OrderBy(c => c.ChapterNumber)
                .FirstOrDefault();

            // Tìm chương có số thứ tự nhỏ hơn gần nhất (Chương trước)
            var prevChapter = allChapters
                .Where(c => c.ChapterNumber < chapter.ChapterNumber)
                .OrderByDescending(c => c.ChapterNumber)
                .FirstOrDefault();

            // Gửi ID của chương lân cận sang View
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