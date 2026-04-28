using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Features.ProductImages
{
    public record ProductImageDto(Guid Id, string Url, string OriginalFileName, long FileSizeBytes,
        string? AltText, int DisplayOrder);

}
