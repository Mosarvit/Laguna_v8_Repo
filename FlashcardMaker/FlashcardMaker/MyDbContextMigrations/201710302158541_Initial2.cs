namespace FlashcardMaker.MyDbContextMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Flashcards", "starttime", c => c.Long(nullable: false));
            AddColumn("dbo.Flashcards", "endtime", c => c.Long(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Flashcards", "endtime");
            DropColumn("dbo.Flashcards", "starttime");
        }
    }
}
