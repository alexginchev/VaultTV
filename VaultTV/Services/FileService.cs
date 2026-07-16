namespace VaultTV.Services;

public class FileService
{
    private readonly IWebHostEnvironment _env;
    private readonly string _uploadsRoot;

    public FileService(IWebHostEnvironment env)
    {
        _env = env;
        _uploadsRoot = Path.Combine(_env.ContentRootPath, "uploads");
        Directory.CreateDirectory(Path.Combine(_uploadsRoot, "posters"));
        Directory.CreateDirectory(Path.Combine(_uploadsRoot, "backdrops"));
    }

    public async Task<string> SaveImageAsync(IFormFile file, string subfolder)
    {
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(ext))
            throw new InvalidOperationException("Only jpg, jpeg, png, and webp images are allowed.");

        var fileName = $"{Guid.NewGuid()}{ext}";
        var folderPath = Path.Combine(_uploadsRoot, subfolder);
        var filePath = Path.Combine(folderPath, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return $"/uploads/{subfolder}/{fileName}";
    }

    public void DeleteImage(string? relativeUrl)
    {
        if (string.IsNullOrEmpty(relativeUrl)) return;
        var fullPath = Path.Combine(_env.ContentRootPath, relativeUrl.TrimStart('/'));
        if (File.Exists(fullPath)) File.Delete(fullPath);
    }
}