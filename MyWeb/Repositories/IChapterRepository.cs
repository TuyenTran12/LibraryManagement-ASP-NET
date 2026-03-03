using MyWeb.Models;

namespace MyWeb.Repositories
{
    public interface IChapterRepository
    {
        Task AddChapterAsync(Chapter chapter);
        Task<Chapter?> GetChapterByIdAsync(int id);
        Task<List<Chapter>> GetChaptersByBookIdAsync(int bookId);
        Task DeleteChapterAsync(int id);
    }
}