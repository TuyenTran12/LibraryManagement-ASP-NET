using Microsoft.EntityFrameworkCore;
using MyWeb.Data;
using MyWeb.Models;

namespace MyWeb.Repositories
{
    public class ChapterRepository : IChapterRepository
    {
        private readonly MyAppContext _context;

        public ChapterRepository(MyAppContext context)
        {
            _context = context;
        }

        public async Task AddChapterAsync(Chapter chapter)
        {
            await _context.Chapters.AddAsync(chapter);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Chapter>> GetChaptersByBookIdAsync(int bookId)
        {
            return await _context.Chapters
              .Include(c => c.Images)
              .Where(c => c.BookId == bookId)
              .OrderByDescending(c => c.ChapterNumber) // Bạn đang dùng OrderByDescending
                      .ToListAsync();
        }

        public async Task<Chapter?> GetChapterWithImagesAsync(int chapterId)
        {
            return await _context.Chapters
             .Include(c => c.Images)
             .FirstOrDefaultAsync(c => c.Id == chapterId);
        }

        public async Task DeleteChapterAsync(int id)
        {
            var chapter = await GetChapterWithImagesAsync(id);
            if (chapter != null)
            {
                // Nếu xóa Chapter, phải xóa ChapterImages (vì Cascade)
                if (chapter.Images != null && chapter.Images.Any())
                {
                    _context.ChapterImages.RemoveRange(chapter.Images);
                }

                _context.Chapters.Remove(chapter);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<ChapterImage?> GetImageByIdAsync(int imageId)
        {
            return await _context.ChapterImages.FindAsync(imageId);
        }

        public async Task DeleteImageAsync(ChapterImage image)
        {
            int chapterId = image.ChapterId;

            _context.ChapterImages.Remove(image);
            await _context.SaveChangesAsync();

            var remainingImages = await _context.ChapterImages
        .Where(i => i.ChapterId == chapterId)
        .OrderBy(i => i.PageNumber)
        .ToListAsync();

            for (int i = 0; i < remainingImages.Count; i++)
            {
                remainingImages[i].PageNumber = i + 1;
            }

            await _context.SaveChangesAsync();
        }

        public async Task UpdateImageOrderAsync(int chapterId, List<int> imageIds)
        {
            var images = await _context.ChapterImages
              .Where(i => i.ChapterId == chapterId)
              .ToListAsync();

            for (int i = 0; i < imageIds.Count; i++)
            {
                var img = images.FirstOrDefault(x => x.Id == imageIds[i]);
                if (img != null)
                {
                    img.PageNumber = i + 1;
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task AddImagesAsync(List<ChapterImage> images)
        {
            await _context.ChapterImages.AddRangeAsync(images);
            await _context.SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}