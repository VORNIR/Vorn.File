using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using System.IO;
using Vorn.Files.Host.Options;
using Vorn.Files.Host.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
VornOptions vornOptions = new();
IConfigurationSection section = builder.Configuration.GetSection(VornOptions.Section);
section.Bind(vornOptions);
builder.Services.Configure<VornOptions>(section);
builder.Services.AddSingleton<FileInformationsService>();
builder.Services.AddHostedService<HostService>();
builder.Services.AddScoped<RepositoryService>();
builder.Services.AddSingleton<IContentTypeProvider, ContentTypeProvider>();
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
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = vornOptions.Aaas.Authority;
        //options.Audience = vornOptions.Aaas.Resource;
        options.TokenValidationParameters.ValidateAudience = false;
        options.RequireHttpsMetadata = true;
        options.TokenValidationParameters.ValidTypes = ["at+jwt"];
    });
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireClaim("scope", "files")
        .Build();
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
WebApplication app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseCors();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), vornOptions.Files.UploadPath)),
    RequestPath = $"/{vornOptions.Files.UploadPath}",
    ContentTypeProvider = app.Services.GetRequiredService<IContentTypeProvider>(),
});
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
//app.UseStaticFiles();
app.Run();
