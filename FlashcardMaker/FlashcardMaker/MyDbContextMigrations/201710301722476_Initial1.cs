namespace FlashcardMaker.MyDbContextMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial1 : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.SubtitleLines", new[] { "Position", "MovieFileName" }, unique: true, name: "IX_FirstAndSecond");
        }
        
        public override void Down()
        {
            DropIndex("dbo.SubtitleLines", "IX_FirstAndSecond");
        }
    }
}
