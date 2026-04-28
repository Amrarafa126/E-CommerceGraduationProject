using E_Commerce.Service.Interfase;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Service.Repostoiry
{
    public class LocalFileStorageService(IConfiguration config) : IFileStorageService
    {
        private readonly string _basePath = config["FileStorage:BasePath"] ?? "wwwroot/uploads";
        private readonly string _baseUrl = config["FileStorage:BaseUrl"] ?? "/uploads";

        public async Task<string> UploadAsync(
            Stream stream, string fileName, string folder, CancellationToken ct = default)
        {
            var dir = Path.Combine(_basePath, folder);
            Directory.CreateDirectory(dir);

            var unique = $"{Guid.NewGuid()}{Path.GetExtension(fileName)}";
            var filePath = Path.Combine(dir, unique);

            await using var fs = new FileStream(filePath, FileMode.Create);
            await stream.CopyToAsync(fs, ct);
            return $"{_baseUrl}/{folder}/{unique}";
        }

        public Task DeleteAsync(string fileUrl, CancellationToken ct = default)
        {
            var path = fileUrl.Replace(_baseUrl, _basePath);
            if (File.Exists(path)) File.Delete(path);
            return Task.CompletedTask;
        }
    }

}
