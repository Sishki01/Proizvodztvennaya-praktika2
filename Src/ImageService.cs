using ImageApi.Data;
using ImageApi.Models;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;

namespace ImageApi.Services;

public class ImageService
{
    private readonly AppDbContext _db;

    public ImageService(AppDbContext db)
    {
        _db = db;
    }

    // ── POST api/image/add ──────────────────────────────────────────────────

    public async Task<ImageRecord> AddImageAsync(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException("File not found.", filePath);

        var fileInfo = new FileInfo(filePath);
        string ext = fileInfo.Extension.TrimStart('.').ToLower();

        using var image = await Image.LoadAsync(filePath);
        int width = image.Width;
        int height = image.Height;

        var record = new ImageRecord
        {
            Name = Path.GetFileNameWithoutExtension(filePath),
            Size = fileInfo.Length,
            Width = width,
            Height = height,
            Type = ext,
            DateAdded = DateTime.UtcNow,
            FilePath = Path.GetFullPath(filePath)
        };

        _db.Images.Add(record);
        await _db.SaveChangesAsync();
        return record;
    }

    // ── GET api/image/info ──────────────────────────────────────────────────

    public async Task<ImageInfoResponse> GetInfoAsync(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException("File not found.", filePath);

        string fullPath = Path.GetFullPath(filePath);
        var fileInfo = new FileInfo(fullPath);
        string ext = fileInfo.Extension.TrimStart('.').ToLower();

        using var image = await Image.LoadAsync(fullPath);

        // Try to get date from DB; fall back to file creation time
        var record = await _db.Images.FirstOrDefaultAsync(r => r.FilePath == fullPath);
        DateTime dateAdded = record?.DateAdded ?? fileInfo.CreationTimeUtc;

        return new ImageInfoResponse
        {
            Id = record?.Id ?? 0,
            Name = Path.GetFileNameWithoutExtension(fullPath),
            SizeBytes = fileInfo.Length,
            Width = image.Width,
            Height = image.Height,
            Type = ext,
            DateAdded = dateAdded,
            FilePath = fullPath
        };
    }

    // ── PUT api/image/change/name ───────────────────────────────────────────

    public async Task<ImageRecord> RenameImageAsync(string filePath, string newName)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException("File not found.", filePath);

        string fullPath = Path.GetFullPath(filePath);
        string dir = Path.GetDirectoryName(fullPath)!;
        string ext = Path.GetExtension(fullPath);
        string newFullPath = Path.Combine(dir, newName + ext);

        if (File.Exists(newFullPath))
            throw new InvalidOperationException($"A file named '{newName + ext}' already exists in the same directory.");

        File.Move(fullPath, newFullPath);

        // Update DB record if it exists
        var record = await _db.Images.FirstOrDefaultAsync(r => r.FilePath == fullPath);
        if (record != null)
        {
            record.Name = newName;
            record.FilePath = newFullPath;
        }
        else
        {
            var fi = new FileInfo(newFullPath);
            using var img = await Image.LoadAsync(newFullPath);
            record = new ImageRecord
            {
                Name = newName,
                Size = fi.Length,
                Width = img.Width,
                Height = img.Height,
                Type = fi.Extension.TrimStart('.').ToLower(),
                DateAdded = DateTime.UtcNow,
                FilePath = newFullPath
            };
            _db.Images.Add(record);
        }

        await _db.SaveChangesAsync();
        return record;
    }

    // ── GET api/image ───────────────────────────────────────────────────────

    public async Task<List<ImageRecord>> GetAllAsync()
    {
        return await _db.Images.OrderByDescending(r => r.DateAdded).ToListAsync();
    }
}
