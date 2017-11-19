namespace FlashcardMaker.MyDbContextMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial2 : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.SubtitleLines", "IX_FirstAndSecond");
        }
        
        public override void Down()
        {
            CreateIndex("dbo.SubtitleLines", new[] { "Position", "MovieFileName" }, unique: true, name: "IX_FirstAndSecond");
        }
    }
}
