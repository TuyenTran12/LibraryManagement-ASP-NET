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
        public BooksController(IBookRepository bookRepository, IStringLocalizer<SharedResource> localizer, IPhotoService photoService)
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
            // Bỏ qua kiểm tra cột CoverImageUrl và Category (vì EF Core đôi khi tự bắt lỗi 2 cái này)
            ModelState.Remove("CoverImageUrl");
            ModelState.Remove("Category");

            if (ModelState.IsValid)
            {
                // 1. Nếu Admin có chọn file ảnh
                if (coverImage != null && coverImage.Length > 0)
                {
                    // 2. Gọi hàm đẩy ảnh lên Cloud
                    var imageResult = await _photoService.AddPhotoAsync(coverImage);

                    // 3. Lấy cái Link ảnh (SecureUrl) lưu vào object Book
                    book.CoverImageUrl = imageResult.SecureUrl.ToString();
                }
                else
                {
                    // Nếu không chọn ảnh thì gán 1 ảnh mặc định
                    book.CoverImageUrl = "https://example.com/default-book.png";
                }

                // 4. Lưu Book vào Database
                await _bookRepository.AddBookAsync(book);
                return RedirectToAction(nameof(Index));
            }

            // Nếu lỗi, phải load lại CategoryList để view hiển thị lại dropdown list
            return View(book);
        }
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var book = await _bookRepository.GetBookByIdAsync(id);

            // THÊM DÒNG NÀY: Nếu tìm không thấy sách thì báo lỗi 404 (Not Found)
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
            // 1. Kiểm tra ID trên URL và ID của sách gửi lên có khớp nhau không
            if (id != book.Id)
            {
                return NotFound();
            }

            // 2. Bỏ qua kiểm tra lỗi rỗng của các trường này (vì ta sẽ tự xử lý hoặc không cần thiết)
            ModelState.Remove("CoverImageUrl");
            ModelState.Remove("Category");

            if (ModelState.IsValid)
            {
                try
                {
                    // 3. Nếu Admin CÓ CHỌN file ảnh mới từ máy tính
                    if (coverImage != null && coverImage.Length > 0)
                    {
                        // Gọi Service đẩy ảnh mới lên Cloudinary
                        var imageResult = await _photoService.AddPhotoAsync(coverImage);

                        // Lấy link ảnh mới gán vào object Book
                        book.CoverImageUrl = imageResult.SecureUrl.ToString();
                    }
                    // LƯU Ý: Nếu coverImage == null, ta không làm gì cả. 
                    // Lúc này biến book.CoverImageUrl vẫn mang giá trị của link ảnh cũ 
                    // (vì HTML đã gửi nó về qua thẻ <input type="hidden">)

                    // 4. Cập nhật thông tin sách vào Database
                    await _bookRepository.UpdateBookAsync(book);

                    // 5. Quay về trang danh sách
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    // Bắt lỗi nếu Database có vấn đề
                    ModelState.AddModelError("", "Đã xảy ra lỗi khi lưu: " + ex.Message);
                }
            }

            // 6. QUAN TRỌNG: Nếu form bị lỗi (nhập thiếu tên, năm...), 
            // phải lấy lại danh sách Categories để hiển thị lại Dropdown, nếu không web sẽ báo lỗi rỗng.
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