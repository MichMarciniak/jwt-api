using FileUploader.Data;
using FileUploader.Entities;

namespace FileUploader.Services;

public class FileService
{
    private readonly AppDbContext _context;

    public FileService(AppDbContext context)
    {
        _context = context;
    }

    public async Task SaveFileRecord(FileEntity file)
    {
        _context.Files.Add(file);
        await _context.SaveChangesAsync();
    }
}