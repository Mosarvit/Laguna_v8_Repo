namespace FlashcardMaker.MyDbContextMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ChineseCharacters",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Chinese = c.String(unicode: false),
                        Rank = c.Int(nullable: false),
                        KnowLevel = c.Int(nullable: false),
                        PinYin = c.String(unicode: false),
                        English = c.String(unicode: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ChineseWords",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        position = c.Int(nullable: false),
                        Chinese = c.String(unicode: false),
                        knowLevel = c.Int(nullable: false),
                        PinYin = c.String(unicode: false),
                        English = c.String(unicode: false),
                        AddedToTempSort = c.Boolean(nullable: false),
                        numberOfFlashcardsItsIn = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SubtitleLinePacks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        NumberOfNotYetInTempSortWords = c.Int(nullable: false),
                        NumberOfCharacters = c.Int(nullable: false),
                        NumberOfToLearnWords = c.Int(nullable: false),
                        DensityOfToLearnWords = c.Double(nullable: false),
                        AddedToFlashcards = c.Boolean(nullable: false),
                        EndTime = c.Int(nullable: false),
                        StartTime = c.Int(nullable: false),
                        MediaFileSegments_remote_id = c.Int(nullable: false),
                        Movie_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Movies", t => t.Movie_Id)
                .Index(t => t.Movie_Id);
            
            CreateTable(
                "dbo.Flashcards",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        question = c.String(unicode: false),
                        duetime = c.Long(nullable: false),
                        MediaFileSegment_remote_id = c.Int(nullable: false),
                        remote_id = c.Int(nullable: false),
                        utserverwhenloaded = c.Long(nullable: false),
                        utlocal = c.Long(nullable: false),
                        toDelete = c.Boolean(nullable: false),
                        isNew = c.Boolean(nullable: false),
                        SubtitleLinePack_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.SubtitleLinePacks", t => t.SubtitleLinePack_Id)
                .Index(t => t.SubtitleLinePack_Id);
            
            CreateTable(
                "dbo.Movies",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        fileName = c.String(maxLength: 100, storeType: "nvarchar"),
                        fullFileName = c.String(unicode: false),
                        fileExtention = c.String(unicode: false),
                        added = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SubtitleLines",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Position = c.Int(nullable: false),
                        MovieFileName = c.String(maxLength: 100, storeType: "nvarchar"),
                        Chinese = c.String(unicode: false),
                        TimeFrameString = c.String(unicode: false),
                        CanRead = c.Int(nullable: false),
                        starttime = c.Int(nullable: false),
                        endtime = c.Int(nullable: false),
                        NumberOfCharacters = c.Int(nullable: false),
                        NumberOfToLearnWords = c.Int(nullable: false),
                        Movie_Id = c.Int(),
                        SubtitleLinePack_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Movies", t => t.Movie_Id)
                .ForeignKey("dbo.SubtitleLinePacks", t => t.SubtitleLinePack_Id)
                .Index(t => new { t.Position, t.MovieFileName }, unique: true, name: "IX_FirstAndSecond")
                .Index(t => t.Movie_Id)
                .Index(t => t.SubtitleLinePack_Id);
            
            CreateTable(
                "dbo.MediaFiles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FileName = c.String(unicode: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.MediaFileSegments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MediaFileName = c.String(maxLength: 100, storeType: "nvarchar"),
                        FileName = c.String(maxLength: 100, storeType: "nvarchar"),
                        remote_id = c.Int(nullable: false),
                        utserverwhenloaded = c.Long(nullable: false),
                        utlocal = c.Long(nullable: false),
                        toDelete = c.Boolean(nullable: false),
                        isNew = c.Boolean(nullable: false),
                        MediaFile_Id = c.Int(),
                        SubtitleLinePack_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.MediaFiles", t => t.MediaFile_Id)
                .ForeignKey("dbo.SubtitleLinePacks", t => t.SubtitleLinePack_Id)
                .Index(t => new { t.MediaFileName, t.FileName }, unique: true, name: "IX_FirstAndSecond")
                .Index(t => t.MediaFile_Id)
                .Index(t => t.SubtitleLinePack_Id);
            
            CreateTable(
                "dbo.NoneChineseCharacters",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Character = c.String(maxLength: 10, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ChineseWordChineseCharacters",
                c => new
                    {
                        ChineseWord_Id = c.Int(nullable: false),
                        ChineseCharacter_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ChineseWord_Id, t.ChineseCharacter_Id })
                .ForeignKey("dbo.ChineseWords", t => t.ChineseWord_Id, cascadeDelete: true)
                .ForeignKey("dbo.ChineseCharacters", t => t.ChineseCharacter_Id, cascadeDelete: true)
                .Index(t => t.ChineseWord_Id)
                .Index(t => t.ChineseCharacter_Id);
            
            CreateTable(
                "dbo.SubtitleLinePackChineseWords",
                c => new
                    {
                        SubtitleLinePack_Id = c.Int(nullable: false),
                        ChineseWord_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.SubtitleLinePack_Id, t.ChineseWord_Id })
                .ForeignKey("dbo.SubtitleLinePacks", t => t.SubtitleLinePack_Id, cascadeDelete: true)
                .ForeignKey("dbo.ChineseWords", t => t.ChineseWord_Id, cascadeDelete: true)
                .Index(t => t.SubtitleLinePack_Id)
                .Index(t => t.ChineseWord_Id);
            
            CreateTable(
                "dbo.SubtitleLineChineseWords",
                c => new
                    {
                        SubtitleLine_Id = c.Int(nullable: false),
                        ChineseWord_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.SubtitleLine_Id, t.ChineseWord_Id })
                .ForeignKey("dbo.SubtitleLines", t => t.SubtitleLine_Id, cascadeDelete: true)
                .ForeignKey("dbo.ChineseWords", t => t.ChineseWord_Id, cascadeDelete: true)
                .Index(t => t.SubtitleLine_Id)
                .Index(t => t.ChineseWord_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MediaFileSegments", "SubtitleLinePack_Id", "dbo.SubtitleLinePacks");
            DropForeignKey("dbo.MediaFileSegments", "MediaFile_Id", "dbo.MediaFiles");
            DropForeignKey("dbo.SubtitleLines", "SubtitleLinePack_Id", "dbo.SubtitleLinePacks");
            DropForeignKey("dbo.SubtitleLineChineseWords", "ChineseWord_Id", "dbo.ChineseWords");
            DropForeignKey("dbo.SubtitleLineChineseWords", "SubtitleLine_Id", "dbo.SubtitleLines");
            DropForeignKey("dbo.SubtitleLines", "Movie_Id", "dbo.Movies");
            DropForeignKey("dbo.SubtitleLinePacks", "Movie_Id", "dbo.Movies");
            DropForeignKey("dbo.Flashcards", "SubtitleLinePack_Id", "dbo.SubtitleLinePacks");
            DropForeignKey("dbo.SubtitleLinePackChineseWords", "ChineseWord_Id", "dbo.ChineseWords");
            DropForeignKey("dbo.SubtitleLinePackChineseWords", "SubtitleLinePack_Id", "dbo.SubtitleLinePacks");
            DropForeignKey("dbo.ChineseWordChineseCharacters", "ChineseCharacter_Id", "dbo.ChineseCharacters");
            DropForeignKey("dbo.ChineseWordChineseCharacters", "ChineseWord_Id", "dbo.ChineseWords");
            DropIndex("dbo.SubtitleLineChineseWords", new[] { "ChineseWord_Id" });
            DropIndex("dbo.SubtitleLineChineseWords", new[] { "SubtitleLine_Id" });
            DropIndex("dbo.SubtitleLinePackChineseWords", new[] { "ChineseWord_Id" });
            DropIndex("dbo.SubtitleLinePackChineseWords", new[] { "SubtitleLinePack_Id" });
            DropIndex("dbo.ChineseWordChineseCharacters", new[] { "ChineseCharacter_Id" });
            DropIndex("dbo.ChineseWordChineseCharacters", new[] { "ChineseWord_Id" });
            DropIndex("dbo.MediaFileSegments", new[] { "SubtitleLinePack_Id" });
            DropIndex("dbo.MediaFileSegments", new[] { "MediaFile_Id" });
            DropIndex("dbo.MediaFileSegments", "IX_FirstAndSecond");
            DropIndex("dbo.SubtitleLines", new[] { "SubtitleLinePack_Id" });
            DropIndex("dbo.SubtitleLines", new[] { "Movie_Id" });
            DropIndex("dbo.SubtitleLines", "IX_FirstAndSecond");
            DropIndex("dbo.Flashcards", new[] { "SubtitleLinePack_Id" });
            DropIndex("dbo.SubtitleLinePacks", new[] { "Movie_Id" });
            DropTable("dbo.SubtitleLineChineseWords");
            DropTable("dbo.SubtitleLinePackChineseWords");
            DropTable("dbo.ChineseWordChineseCharacters");
            DropTable("dbo.NoneChineseCharacters");
            DropTable("dbo.MediaFileSegments");
            DropTable("dbo.MediaFiles");
            DropTable("dbo.SubtitleLines");
            DropTable("dbo.Movies");
            DropTable("dbo.Flashcards");
            DropTable("dbo.SubtitleLinePacks");
            DropTable("dbo.ChineseWords");
            DropTable("dbo.ChineseCharacters");
        }
    }
}
