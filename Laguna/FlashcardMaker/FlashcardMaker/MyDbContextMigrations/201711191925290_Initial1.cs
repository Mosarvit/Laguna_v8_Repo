namespace FlashcardMaker.MyDbContextMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial1 : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.SubtitleLines", "IX_FirstAndSecond");
            AlterColumn("dbo.SubtitleLines", "MovieFileName", c => c.String(maxLength: 200, storeType: "nvarchar"));
            CreateIndex("dbo.SubtitleLines", new[] { "Position", "MovieFileName" }, unique: true, name: "IX_FirstAndSecond");
        }
        
        public override void Down()
        {
            DropIndex("dbo.SubtitleLines", "IX_FirstAndSecond");
            AlterColumn("dbo.SubtitleLines", "MovieFileName", c => c.String(maxLength: 100, storeType: "nvarchar"));
            CreateIndex("dbo.SubtitleLines", new[] { "Position", "MovieFileName" }, unique: true, name: "IX_FirstAndSecond");
        }
    }
}
