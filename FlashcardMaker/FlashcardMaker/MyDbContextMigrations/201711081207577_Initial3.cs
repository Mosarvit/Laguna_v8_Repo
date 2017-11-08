namespace FlashcardMaker.MyDbContextMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SubtitleLinePacks", "importance", c => c.Double(nullable: false));
            DropColumn("dbo.SubtitleLinePacks", "DensityOfToLearnWords");
        }
        
        public override void Down()
        {
            AddColumn("dbo.SubtitleLinePacks", "DensityOfToLearnWords", c => c.Double(nullable: false));
            DropColumn("dbo.SubtitleLinePacks", "importance");
        }
    }
}
