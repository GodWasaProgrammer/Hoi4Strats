namespace SharedProj.DBModels;

public class GuideModel
{
    public required int Id { get; set; } // Primärnyckel
    public required string Title { get; set; } // Guidens titel
    public required string Content { get; set; } // Guidens innehåll
    public required string Author { get; set; } // Namn eller ID för författaren
    public DateTime CreatedAt { get; set; } // När guiden skapades
    public List<ImageModel>? Pictures { get; set; } = [];
    public Review Status { get; set; } = Review.UnderReview;
    public required GuideTypes GuideType { get; set; }
}