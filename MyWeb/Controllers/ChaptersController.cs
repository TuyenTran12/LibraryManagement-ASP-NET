using Microsoft.AspNetCore.Mvc;
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

        public ChaptersController(IChapterRepository chapterRepository,
                                  IPhotoService photoService,
                                  IBookRepository bookRepository)
        {
            _chapterRepository = chapterRepository;
            _photoService = photoService;
            _bookRepository = bookRepository;
        }

        [HttpGet]
        public async Task<IActionResult> UploadChapter(int bookId)
        {
            var book = await _bookRepository.GetBookByIdAsync(bookId);
            if (book == null) return NotFound("Không tìm thấy truyện này!");

            ViewBag.BookTitle = book.Title;
            ViewBag.BookId = book.Id;
            ViewBag.ExistingChapters = await _chapterRepository.GetChaptersByBookIdAsync(bookId);

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadChapter(int bookId, double chapterNumber, string title, List<IFormFile> pages)
        {
            if (pages == null || pages.Count == 0)
            {
                ModelState.AddModelError("", "Vui lòng chọn ít nhất 1 trang truyện!");
                ViewBag.BookId = bookId;
                return View();
            }

            var chapter = new Chapter
            {
                BookId = bookId,
                ChapterNumber = chapterNumber,
                Title = title ?? $"Chương {chapterNumber}",
                Images = new List<ChapterImage>()
            };

            int pageIndex = 1;

            foreach (var file in pages.OrderBy(f => f.FileName))
            {
                if (file.Length > 0)
                {
                    var uploadResult = await _photoService.AddPhotoAsync(file);

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
        public async Task<IActionResult> AddExtraPages(int chapterId, List<IFormFile> pages)
        {
            if (pages == null || pages.Count == 0)
                return Json(new { success = false, message = "Chưa có ảnh nào được chọn!" });

            var chapter = await _chapterRepository.GetChapterWithImagesAsync(chapterId);
            if (chapter == null)
                return Json(new { success = false, message = "Không tìm thấy chương này!" });

            int nextPageNumber = 1;
            if (chapter.Images != null && chapter.Images.Any())
            {
                nextPageNumber = chapter.Images.Max(i => i.PageNumber) + 1;
            }

            var newImages = new List<ChapterImage>();

            foreach (var file in pages.OrderBy(f => f.FileName))
            {
                if (file.Length > 0)
                {
                    var uploadResult = await _photoService.AddPhotoAsync(file);

                    newImages.Add(new ChapterImage
                    {
                        ChapterId = chapterId,
                        ImageUrl = uploadResult.SecureUrl.ToString(),
                        PageNumber = nextPageNumber
                    });

                    nextPageNumber++;
                }
            }

            await _chapterRepository.AddImagesAsync(newImages);

            return Json(new { success = true });
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
            var image = await _chapterRepository.GetImageByIdAsync(imageId);
            if (image == null) return Json(new { success = false, message = "Không tìm thấy ảnh!" });

            var publicId = ExtractPublicIdFromUrl(image.ImageUrl);
            if (!string.IsNullOrEmpty(publicId))
            {
                await _photoService.DeletePhotoAsync(publicId);
            }

            await _chapterRepository.DeleteImageAsync(image);

            return Json(new { success = true, message = "Đã xóa ảnh trên Cloud và cập nhật lại số trang!" });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteEntireChapter(int chapterId)
        {
            var chapter = await _chapterRepository.GetChapterWithImagesAsync(chapterId);

            if (chapter == null)
                return Json(new { success = false, message = "Lỗi: Không tìm thấy chương!" });

            if (chapter.Images != null && chapter.Images.Any())
            {
                foreach (var img in chapter.Images)
                {
                    var publicId = ExtractPublicIdFromUrl(img.ImageUrl);
                    if (!string.IsNullOrEmpty(publicId))
                    {
                        await _photoService.DeletePhotoAsync(publicId);
                    }
                }
            }

            await _chapterRepository.DeleteChapterAsync(chapterId);

            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateImageOrder(int chapterId, [FromBody] List<int> imageIds)
        {
            if (imageIds == null || !imageIds.Any()) return Json(new { success = false });

            await _chapterRepository.UpdateImageOrderAsync(chapterId, imageIds);

            return Json(new { success = true });
        }

        private string ExtractPublicIdFromUrl(string url)
        {
            if (string.IsNullOrEmpty(url)) return null;
            try
            {
                var uri = new Uri(url);
                var path = uri.AbsolutePath;
                var segments = path.Split('/');
                var uploadIndex = Array.IndexOf(segments, "upload");

                var publicIdWithExtension = string.Join("/", segments.Skip(uploadIndex + 2));
                return Path.GetFileNameWithoutExtension(publicIdWithExtension);
            }
            catch
            {
                return null;
            }
        }
    }
}