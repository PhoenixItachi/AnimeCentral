namespace AnimeCentralWeb.Domain
{
    public class Review
    {
        public int Id { get; set; }
        public int AnimeId { get; set; }

        public float Score { get; set; }
        public string Content { get; set; }

        public virtual Anime Anime { get; set; } 
    }
}
