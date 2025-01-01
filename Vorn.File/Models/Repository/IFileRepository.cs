using Vorn.File.Host.Models.Data;
using Vorn.File.Host.Models.Local;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Vorn.File.Host.Models.Repository;

public interface IFileRepository
{
    Task<Result<FileInformation>> AddAsync(FileInformation file);
    Task<Result<IEnumerable<FileInformation>>> GetAllAsync();
    Task<Result<bool>> DeleteAsync(string fileName);
}