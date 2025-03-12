namespace SharedProj.DBModels;
public class ImageModel
{
    public int ImageId { get; set; }
    public required string Name { get; set; }
    public required byte[] Content { get; set; } // Här lagras binära data för bilden
    public required string ContentType { get; set; } // MIME-typ, t.ex. "image/png"
    public DateTime CreatedAt { get; set; }
}
