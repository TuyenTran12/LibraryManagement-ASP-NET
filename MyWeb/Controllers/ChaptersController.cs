using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyWeb.Data;
using MyWeb.Models;
using MyWeb.Repositories;
using MyWeb.Services;

namespace MyWeb.Controllers
{
    public class ChaptersController : Controller
    {
        private readonly IChapterRepository _chapterRepository;
        private readonly IPhotoService _photoService;
        private readonly IBookRepository _bookRepository;

        private readonly MyAppContext _context;

        public ChaptersController(IChapterRepository chapterRepository, IPhotoService photoService, IBookRepository bookRepository, MyAppContext context)
        {
            _chapterRepository = chapterRepository;
            _photoService = photoService;
            _bookRepository = bookRepository;
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> UploadChapter(int bookId)
        {
            var book = await _bookRepository.GetBookByIdAsync(bookId);
            if (book == null) return NotFound("Không tìm thấy truyện này!");

            ViewBag.BookTitle = book.Title;
            ViewBag.BookId = book.Id;

            // Lấy danh sách các chương đã có để hiện ở dưới cùng (Nhớ Include Images)
            // Code này dùng DbContext trực tiếp, bạn có thể chuyển thành Repository
            ViewBag.ExistingChapters = await _context.Chapters
                                                     .Include(c => c.Images)
                                                     .Where(c => c.BookId == bookId)
                                                     .OrderByDescending(c => c.ChapterNumber) // Hiện chương mới nhất lên đầu
                                                     .ToListAsync();

            return View();
        }

        // 2. Xử lý khi Admin bấm nút "Tải Lên"
        [HttpPost]
        public async Task<IActionResult> UploadChapter(int bookId, double chapterNumber, string title, List<IFormFile> pages)
        {
            if (pages == null || pages.Count == 0)
            {
                ModelState.AddModelError("", "Vui lòng chọn ít nhất 1 trang truyện!");
                ViewBag.BookId = bookId;
                return View();
            }

            // Tạo khung Chapter chuẩn bị lưu
            var chapter = new Chapter
            {
                BookId = bookId,
                ChapterNumber = chapterNumber,
                Title = title ?? $"Chương {chapterNumber}",
                Images = new List<ChapterImage>()
            };

            int pageIndex = 1;

            // Lặp qua từng ảnh Admin vừa bôi đen
            foreach (var file in pages.OrderBy(f => f.FileName)) // Sắp xếp theo tên file (01.jpg, 02.jpg...)
            {
                if (file.Length > 0)
                {
                    // Đẩy lên Cloudinary
                    var uploadResult = await _photoService.AddPhotoAsync(file);

                    // Lấy link gắn vào Chapter
                    chapter.Images.Add(new ChapterImage
                    {
                        ImageUrl = uploadResult.SecureUrl.ToString(),
                        PageNumber = pageIndex
                    });

                    pageIndex++;
                }
            }

            await _chapterRepository.AddChapterAsync(chapter);

            return RedirectToAction("Index", "Books");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id, int bookId)
        {
            await _chapterRepository.DeleteChapterAsync(id);

            return RedirectToAction("Index", "Books");
        }
        [HttpPost]
        public async Task<IActionResult> DeleteImage(int imageId)
        {
            // 1. Tìm ảnh cần xóa
            var image = await _context.ChapterImages.FindAsync(imageId);
            if (image == null) return Json(new { success = false, message = "Không tìm thấy ảnh!" });

            int chapterId = image.ChapterId;

            // TODO: Nếu muốn chuẩn 100%, bạn nên gọi _photoService.DeletePhotoAsync(...) để xóa ảnh trên Cloudinary luôn cho đỡ tốn dung lượng mây.

            // 2. Xóa ảnh khỏi Database
            _context.ChapterImages.Remove(image);
            await _context.SaveChangesAsync();

            // 3. Lấy các ảnh còn lại của chương này, sắp xếp theo PageNumber cũ
            var remainingImages = await _context.ChapterImages
                                                .Where(i => i.ChapterId == chapterId)
                                                .OrderBy(i => i.PageNumber)
                                                .ToListAsync();

            // 4. "Reset" lại PageNumber cho liền mạch (1, 2, 3...)
            for (int i = 0; i < remainingImages.Count; i++)
            {
                remainingImages[i].PageNumber = i + 1;
            }

            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Đã xóa và cập nhật lại số trang!" });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteEntireChapter(int chapterId)
        {
            // 1. Tìm chương cần xóa, lấy kèm theo tất cả ảnh của nó
            var chapter = await _context.Chapters
                                        .Include(c => c.Images)
                                        .FirstOrDefaultAsync(c => c.Id == chapterId);

            if (chapter == null)
                return Json(new { success = false, message = "Lỗi: Không tìm thấy chương này trong Database!" });

            // 2. Xóa toàn bộ ảnh của chương này trước (để tránh lỗi khóa ngoại)
            if (chapter.Images != null && chapter.Images.Any())
            {
                _context.ChapterImages.RemoveRange(chapter.Images);
            }

            // 3. Xóa chính chương đó
            _context.Chapters.Remove(chapter);

            // 4. Lưu thay đổi xuống SQL
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }
        // API 2: Cập nhật lại thứ tự khi kéo thả
        [HttpPost]
        public async Task<IActionResult> UpdateImageOrder(int chapterId, [FromBody] List<int> imageIds)
        {
            if (imageIds == null || !imageIds.Any()) return Json(new { success = false });

            // Lấy tất cả ảnh của chương này
            var images = await _context.ChapterImages.Where(i => i.ChapterId == chapterId).ToListAsync();

            // Dựa vào mảng ID gửi từ Javascript (đã theo thứ tự kéo thả), cập nhật lại PageNumber
            for (int i = 0; i < imageIds.Count; i++)
            {
                var img = images.FirstOrDefault(x => x.Id == imageIds[i]);
                if (img != null)
                {
                    img.PageNumber = i + 1; // Số thứ tự mới
                }
            }

            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }
    }
}