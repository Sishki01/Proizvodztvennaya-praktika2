namespace ImageApi.Models;

public class ImageRecord
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public long Size { get; set; }           // bytes
    public int Width { get; set; }           // px
    public int Height { get; set; }          // px
    public string Type { get; set; } = string.Empty;  // png, jpg, etc.
    public DateTime DateAdded { get; set; } = DateTime.UtcNow;
    public string FilePath { get; set; } = string.Empty;
}
