namespace ImageApi.Models;

public class AddImageRequest
{
    /// <summary>Absolute or relative path to the image file on disk.</summary>
    public string FilePath { get; set; } = string.Empty;
}

public class ImageInfoResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public long SizeBytes { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public string Type { get; set; } = string.Empty;
    public DateTime DateAdded { get; set; }
    public string FilePath { get; set; } = string.Empty;
}

public class RenameRequest
{
    /// <summary>Current file path of the image.</summary>
    public string FilePath { get; set; } = string.Empty;
    /// <summary>New name (without extension).</summary>
    public string NewName { get; set; } = string.Empty;
}
