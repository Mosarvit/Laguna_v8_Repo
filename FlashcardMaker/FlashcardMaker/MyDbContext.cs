namespace FlashcardMaker
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using FlashcardMaker.Models;

    public partial class MyDbContext : DbContext
    {
        public DbSet<SubtitleLine> SubtitleLines { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<ChineseCharacter> ChineseCharacters { get; set; }
        public DbSet<NoneChineseCharacter> NoneChineseCharacters { get; set; }
        public DbSet<ChineseWord> ChineseWords { get; set; }
        public DbSet<SubtitleLinePack> SubtitleLinePacks { get; set; }
        public DbSet<Flashcard> Flashcards { get; set; }
        public DbSet<MediaFileSegment> MediaFileSegments { get; set; }
        public DbSet<MediaFile> MediaFiles { get; set; }

        public MyDbContext()
            : base("name=MyDbContext")
        {
            this.Configuration.ProxyCreationEnabled = true;
            this.Configuration.LazyLoadingEnabled = true;
        }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
