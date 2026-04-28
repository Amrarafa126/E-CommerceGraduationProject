using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Service.Interfase
{
    public interface IFileStorageService
    {
        Task<string> UploadAsync(Stream fileStream, string fileName, string folder,CancellationToken ct = default);
        Task DeleteAsync(string fileUrl, CancellationToken ct = default);
    }
}
