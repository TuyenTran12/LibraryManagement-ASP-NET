using Microsoft.AspNetCore.Mvc;
using MyWeb.Models;

namespace MyWeb.Repositories
{
    public interface IBookRepository
    {
        Task<IEnumerable<Book>> GetAllBooksAsync();
        Task<Book?> GetBookByIdAsync(int id);
        Task<IEnumerable<Category>> GetCategoriesAsync();
        Task AddBookAsync(Book book);
        Task UpdateBookAsync(Book book);
        Task DeleteBookAsync(int id);
    }
}
