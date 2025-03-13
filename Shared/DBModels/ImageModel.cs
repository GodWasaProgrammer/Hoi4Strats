namespace SharedProj.DBModels;
public class ImageModel
{
    public int ImageId { get; set; }
    public required string Name { get; set; }
    public required byte[] Content { get; set; } // BLOB
    public required string ContentType { get; set; } // MIME-type
    public DateTime CreatedAt { get; set; }
    
    // FK
    public int GuideId { get; set; }
}
