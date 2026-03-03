using Microsoft.EntityFrameworkCore;
using MyWeb.Data; // Thay bằng thư mục chứa DbContext của bạn
using MyWeb.Models;

namespace MyWeb.Repositories
{
    public class ChapterRepository : IChapterRepository
    {
        private readonly MyAppContext _context; // Thay tên DbContext nếu của bạn khác

        public ChapterRepository(MyAppContext context)
        {
            _context = context;
        }

        public async Task AddChapterAsync(Chapter chapter)
        {
            await _context.Chapters.AddAsync(chapter);
            await _context.SaveChangesAsync();
        }

        public async Task<Chapter?> GetChapterByIdAsync(int id)
        {
            return await _context.Chapters
                .Include(c => c.Images) 
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<List<Chapter>> GetChaptersByBookIdAsync(int bookId)
        {
            return await _context.Chapters
                .Where(c => c.BookId == bookId)
                .OrderBy(c => c.ChapterNumber) // Sắp xếp theo thứ tự chương
                .ToListAsync();
        }
        public async Task DeleteChapterAsync(int id)
        {
            var chapter = await _context.Chapters
                                        .Include(c => c.Images)
                                        .FirstOrDefaultAsync(c => c.Id == id);
            if (chapter != null)
            {
                _context.Chapters.Remove(chapter);
                await _context.SaveChangesAsync();
            }
        }
    }
}