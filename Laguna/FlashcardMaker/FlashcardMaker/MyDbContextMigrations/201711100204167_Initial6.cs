namespace FlashcardMaker.MyDbContextMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial6 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Flashcards", "English");
            DropColumn("dbo.Flashcards", "Translit");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Flashcards", "Translit", c => c.String(unicode: false));
            AddColumn("dbo.Flashcards", "English", c => c.String(unicode: false));
        }
    }
}
