using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyWeb.Data;
using MyWeb.Migrations;
using MyWeb.Models;
using System.Security.Claims;

namespace MyWeb.Repositories
{
    public class BookRepository : IBookRepository
    {
        public readonly MyAppContext _context;
        public BookRepository(MyAppContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Book>> GetAllBooksAsync()
        {
            return await _context.Books
              .Include(c => c.Category)
              .Include(b => b.Chapters)
              .ToListAsync();
        }
        public async Task<Book?> GetBookByIdAsync(int id)
        {
            return await _context.Books
              .Include(c => c.Category)
              .FirstOrDefaultAsync(b => b.Id == id);
        }
        public async Task<IEnumerable<Category>> GetCategoriesAsync()
        {
            return await _context.Categories.ToListAsync();
        }
        public async Task AddBookAsync(Book book)
        {
            _context.Books.Add(book);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateBookAsync(Book book)
        {
            _context.Books.Update(book);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteBookAsync(int id)
        {
            var book = await _context.Books.FirstOrDefaultAsync(b => b.Id == id);

            if (book != null)
            {
                _context.Books.Remove(book);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<IEnumerable<Book>> SearchBooksAsync(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return new List<Book>();
            }

            var lowerKeyword = keyword.ToLower();

            return await _context.Books
                .Include(b => b.Category)
                .Where(b => b.Title.ToLower().Contains(lowerKeyword) ||
                            b.Author.ToLower().Contains(lowerKeyword))
                .OrderByDescending(b => b.Id)
                .ToListAsync();
        }
        public async Task<Book?> GetBookWithDetailsAsync(int id)
        {
            return await _context.Books
                .Include(b => b.Category)
                .Include(b => b.Chapters)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<IEnumerable<Book>> GetSimilarBooksAsync(int categoryId, int excludeBookId, int takeCount = 5)
        {
            return await _context.Books
                .Where(b => b.CategoryId == categoryId && b.Id != excludeBookId)
                .OrderByDescending(b => b.Id)
                .Take(takeCount)
                .ToListAsync();
        }
        public async Task<IActionResult> ToggleBookmark(string userId, int bookId)
        {
            var existingBookmark = await _context.UserBookLibraries
                .FirstOrDefaultAsync(b => b.UserId == userId && b.BookId == bookId);

            if (existingBookmark != null)
            {
                _context.UserBookLibraries.Remove(existingBookmark);
                await _context.SaveChangesAsync();
                return new JsonResult(new { success = true, isBookmarked = false });
            }
            else
            {
                var newBookmark = new UserBookLibrary
                {
                    UserId = userId,
                    BookId = bookId,
                    BookName = (await _context.Books.FindAsync(bookId))?.Title ?? "Unknown"
                };
                _context.UserBookLibraries.Add(newBookmark);
                await _context.SaveChangesAsync();
                return new JsonResult(new { success = true, isBookmarked = true });
            }
        }
    }
}