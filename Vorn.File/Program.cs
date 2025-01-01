using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using System.IO;
using Vorn.File.Host.Models.Repository;
using Vorn.File.Host.Options;
using Vorn.File.Host.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<VornOptions>(builder.Configuration.GetSection(VornOptions.Section));
builder.Services.AddSingleton<FileService>();
builder.Services.AddHostedService<FileHostedService>();
builder.Services.AddScoped<IFileRepository, FileRepository>();
builder.Services.AddScoped<IFileRepositoryService, FileRepositoryService>();
builder.Services.AddDirectoryBrowser();
builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin();
        builder.AllowAnyMethod();
        builder.AllowAnyHeader();
    });
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
WebApplication app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.MapControllers();
app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "uploads")),
    RequestPath = "/uploads"
});
app.Run();
