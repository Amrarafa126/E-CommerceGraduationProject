using E_Commerce.Data.Entity;
using Microsoft.AspNetCore.Http;


namespace E_Commerce.Service.Interfase
{
    public interface IFileService
    {
       public Task<List<string>> UploadProductImages(string location, List<IFormFile> files);
    }
}
