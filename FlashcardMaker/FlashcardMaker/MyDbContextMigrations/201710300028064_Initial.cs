namespace FlashcardMaker.MyDbContextMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Flashcards", new[] { "remote_id" });
        }
        
        public override void Down()
        {
            CreateIndex("dbo.Flashcards", "remote_id", unique: true);
        }
    }
}
