using Microsoft.EntityFrameworkCore;

namespace Vorn.File.Host.Models.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
	public DbSet<FileInformation>? Files { get; set; }
}