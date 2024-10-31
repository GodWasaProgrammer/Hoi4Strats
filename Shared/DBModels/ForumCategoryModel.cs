namespace SharedProj.DBModels;

public class ForumCategoryModel
{
    public int ID { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public List<ForumThreadModel> Threads { get; set; }
}
