using EmployeeManagement.Data;
using EmployeeManagement.Interfaces;
using EmployeeManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Repositories;

public class FileRepository(
    ApplicationDbContext context,
    ILogger<FileRepository> logger) : IFileRepository
{


    public async Task<UploadedFile> AddAsync(UploadedFile file)
    {
        await context.Set<UploadedFile>().AddAsync(file);

        try
        {
            await context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to save file {FileId} ({OriginalFileName}) to the database",
                file.Id, file.OriginalFileName);
            throw;
        }

        return file;
    }

    public async Task<UploadedFile?> GetByIdAsync(Guid id)
    {
        return await context.Set<UploadedFile>().FindAsync(id);
    }

    public async Task DeleteAsync(UploadedFile file)
    {
        context.Set<UploadedFile>().Remove(file);

        try
        {
            await context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to delete file {FileId} from the database", file.Id);
            throw;
        }

    }

    public async Task<(IEnumerable<UploadedFile> Files, int TotalCount)> GetPagedFilesByUserAsync(int userId, int pageNumber, int pageSize)
    {
        var query = context.Set<UploadedFile>()
            .Where(f => f.UserId == userId)
            .OrderByDescending(f => f.CreatedAt);

        var totalCount = await query.CountAsync();

        var files = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (files, totalCount);
    }
}