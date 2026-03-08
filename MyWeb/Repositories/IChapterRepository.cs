using MyWeb.Models;

namespace MyWeb.Repositories
{
    public interface IChapterRepository
    {
        Task AddChapterAsync(Chapter chapter);
        Task<List<Chapter>> GetChaptersByBookIdAsync(int bookId);
        Task DeleteChapterAsync(int id);

        Task<ChapterImage?> GetImageByIdAsync(int imageId);
        Task DeleteImageAsync(ChapterImage image);
        Task UpdateImageOrderAsync(int chapterId, List<int> imageIds);
        Task<Chapter?> GetChapterWithImagesAsync(int chapterId); // Lấy chương kèm danh sách ảnh
        Task AddImagesAsync(List<ChapterImage> images); // Dùng cho AddExtraPages
        Task SaveChangesAsync();
    }
}