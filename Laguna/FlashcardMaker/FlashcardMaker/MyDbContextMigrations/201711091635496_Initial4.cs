namespace FlashcardMaker.MyDbContextMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial4 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MediaFileSegments", "ToUpload", c => c.Boolean(nullable: false));
            AddColumn("dbo.MediaFileSegments", "ToDownload", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.MediaFileSegments", "ToDownload");
            DropColumn("dbo.MediaFileSegments", "ToUpload");
        }
    }
}
