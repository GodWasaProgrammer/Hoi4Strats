namespace Shared.DBModels
{
    public class ForumThreadModel
    {
        // id of thread
        public int Id { get; set; }
        public string Title { get; set; }
        // id of creating User
        public int UserID { get; set; }
        public int ForumCategoryId { get; set; }
        public List<ForumPostModel> Posts { get; set; }
        public string Post { get; set; }
        public DateTime DateTime { get; set; }
    }
}
