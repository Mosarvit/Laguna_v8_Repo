namespace FlashcardMaker.MyDbContextMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial5 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Flashcards", "English", c => c.String(unicode: false));
            AddColumn("dbo.Flashcards", "Translit", c => c.String(unicode: false));
            AddColumn("dbo.SubtitleLines", "English", c => c.String(unicode: false));
            AddColumn("dbo.SubtitleLines", "Translit", c => c.String(unicode: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SubtitleLines", "Translit");
            DropColumn("dbo.SubtitleLines", "English");
            DropColumn("dbo.Flashcards", "Translit");
            DropColumn("dbo.Flashcards", "English");
        }
    }
}
