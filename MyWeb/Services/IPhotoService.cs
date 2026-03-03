using CloudinaryDotNet.Actions;

namespace MyWeb.Services
{
    public interface IPhotoService
    {
        Task<ImageUploadResult> AddPhotoAsync(IFormFile file);
    }
}