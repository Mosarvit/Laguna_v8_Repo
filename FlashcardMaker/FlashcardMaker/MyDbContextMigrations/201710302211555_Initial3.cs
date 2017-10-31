namespace FlashcardMaker.MyDbContextMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SubtitleLines", "starttime", c => c.Long(nullable: false));
            AddColumn("dbo.SubtitleLines", "endtime", c => c.Long(nullable: false));
            DropColumn("dbo.Flashcards", "starttime");
            DropColumn("dbo.Flashcards", "endtime");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Flashcards", "endtime", c => c.Long(nullable: false));
            AddColumn("dbo.Flashcards", "starttime", c => c.Long(nullable: false));
            DropColumn("dbo.SubtitleLines", "endtime");
            DropColumn("dbo.SubtitleLines", "starttime");
        }
    }
}
