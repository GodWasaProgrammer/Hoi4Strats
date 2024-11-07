namespace SharedProj.DBModels;

public class TemplateGuideModel
{
    public string Picture { get; set; } // picture associated with template
    public required int Id { get; set; } // Primärnyckel
    public required string Content { get; set; } // Guidens innehåll
    public required string Author { get; set; } // Namn eller ID för författaren
    public DateTime CreatedAt { get; set; } // När guiden skapades
}

