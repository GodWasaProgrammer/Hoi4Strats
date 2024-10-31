namespace SharedProj.newsitems
{
    public class Appnews
    {
#pragma warning disable IDE1006 // Naming Styles
        public int appid { get; set; }
        public List<Newsitem>? newsitems { get; set; }
        public int count { get; set; }
#pragma warning restore IDE1006 // Naming Styles
    }
}
