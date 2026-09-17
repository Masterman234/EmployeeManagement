using EmployeeManagement.Data;
using EmployeeManagement.Interfaces;
using EmployeeManagement.Models;

namespace EmployeeManagement.Repositories;

public class FileRepository(ApplicationDbContext context) : IFileRepository
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
            Console.WriteLine(ex.ToString());
            throw;
        }

        return file;
    }
    public async Task<UploadedFile?> GetByIdAsync(Guid id)
    {
        return await context.Set<UploadedFile>().FindAsync(id);
    }

    public async Task DeleteAsync(UploadedFile File)
    {
        context.Set<UploadedFile>().Remove(File);
        await context.SaveChangesAsync();
    }

}