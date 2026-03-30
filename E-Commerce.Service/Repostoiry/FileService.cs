using E_Commerce.Service.Interfase;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using E_Commerce.Data.Entity;

namespace E_Commerce.Service.Repostoiry
{
    public class FileService : IFileService
    {
        IWebHostEnvironment GetHostEnvironment;
        public FileService(IWebHostEnvironment _GetHostEnvironment)
        {
            GetHostEnvironment = _GetHostEnvironment;
        }

        public async Task<List<string>> UploadProductImages(string location, List<IFormFile> files)
        {
            var urls = new List<string>();

            if (files == null || !files.Any())
                return urls;

            var folderPath = Path.Combine(GetHostEnvironment.WebRootPath, location);

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            foreach (var file in files)
            {
                if (file.Length == 0) continue;

                if (!file.ContentType.StartsWith("image/"))
                    continue;

                var extension = Path.GetExtension(file.FileName);
                var fileName = $"{Guid.NewGuid():N}{extension}";
                var fullPath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                urls.Add($"/{location}/{fileName}");
                 
            }

            return urls;
        }
        //public async Task<List<ProductImage>> UploadProductImages(string location, List<IFormFile> files)
        //{
        //    var images = new List<ProductImage>();

        //    if (files == null || !files.Any())
        //        return images;

        //    var folderPath = Path.Combine(GetHostEnvironment.WebRootPath, location/*, companyId.ToString()*/);

        //    if (!Directory.Exists(folderPath))
        //        Directory.CreateDirectory(folderPath);

        //    foreach (var file in files)
        //    {
        //        if (file.Length == 0) continue;

        //        if (!file.ContentType.StartsWith("image/"))
        //            continue;

        //        var extension = Path.GetExtension(file.FileName);
        //        var fileName = $"{Guid.NewGuid():N}{extension}";
        //        var fullPath = Path.Combine(folderPath, fileName);

        //        using (var stream = new FileStream(fullPath, FileMode.Create))
        //        {
        //            await file.CopyToAsync(stream);
        //        }
        //        images.Add(new ProductImage
        //        {
        //            ImageUrl = $"/{location}/{fileName}",
        //            IsMain = false
        //        });

        //    }

        //    return images;
        //}


    }
    }
