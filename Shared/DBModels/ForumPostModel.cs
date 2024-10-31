namespace SharedProj.DBModels;

public class ForumPostModel
{
    public int Id { get; set; }
    public int ThreadID { get; set; }
    public int UserID { get; set; }
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; }
}
