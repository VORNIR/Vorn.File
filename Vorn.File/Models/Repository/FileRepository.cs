using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vorn.File.Host.Models.Data;
using Vorn.File.Host.Models.Local;
using Vorn.File.Host.Services;

namespace Vorn.File.Host.Models.Repository;

public class FileRepository(FileService fileService) : IFileRepository
{
    public async Task<Result<FileInformation>> AddAsync(FileInformation imageFile)
    {
        fileService.FilesList.Add(imageFile);
        await fileService.Save();
        return new Result<FileInformation>(imageFile);
    }

    public async Task<Result<bool>> DeleteAsync(string fileId)
    {
        FileInformation? file = fileService.FilesList.SingleOrDefault(x => x.Id == fileId);
        if(file == null)
            return new Result<bool>(false, "File record not found.");

        fileService.FilesList.Remove(file);
        await fileService.Save();

        return new Result<bool>(true);
    }

    public async Task<Result<IEnumerable<FileInformation>>> GetAllAsync()
    {
        List<FileInformation> files = fileService.FilesList;
        return new Result<IEnumerable<FileInformation>>(files);
    }
}