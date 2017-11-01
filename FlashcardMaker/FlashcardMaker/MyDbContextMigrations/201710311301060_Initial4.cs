namespace FlashcardMaker.MyDbContextMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial4 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.SubtitleLines", "starttime", c => c.Int(nullable: false));
            AlterColumn("dbo.SubtitleLines", "endtime", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.SubtitleLines", "endtime", c => c.Long(nullable: false));
            AlterColumn("dbo.SubtitleLines", "starttime", c => c.Long(nullable: false));
        }
    }
}
