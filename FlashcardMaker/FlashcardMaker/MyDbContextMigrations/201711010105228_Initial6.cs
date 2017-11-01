namespace FlashcardMaker.MyDbContextMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial6 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SubtitleLinePacks", "Movie_id", c => c.Int());
            CreateIndex("dbo.SubtitleLinePacks", "Movie_id");
            AddForeignKey("dbo.SubtitleLinePacks", "Movie_id", "dbo.Movies", "id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SubtitleLinePacks", "Movie_id", "dbo.Movies");
            DropIndex("dbo.SubtitleLinePacks", new[] { "Movie_id" });
            DropColumn("dbo.SubtitleLinePacks", "Movie_id");
        }
    }
}
