using Health.Application.DTOs.File;
using System.Threading;
using System.Threading.Tasks;

namespace Health.Application.Interfaces
{
    public interface IFileStorageService
    {
        Task<FileDownloadDto> DownloadAsync(int fileStorageId, CancellationToken ct = default);
    }
}
