using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyWeb.Data;
using MyWeb.Models;
using static System.Reflection.Metadata.BlobBuilder;

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

    }
}
