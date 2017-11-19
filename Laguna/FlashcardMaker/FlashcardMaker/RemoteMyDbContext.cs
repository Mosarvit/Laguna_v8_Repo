namespace FlashcardMaker
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using FlashcardMaker.Models;

    [DbConfigurationType(typeof(MySql.Data.Entity.MySqlEFConfiguration))]
    public partial class RemoteMyDbContext : DbContext
    {
        public DbSet<TestModel> TestModels { get; set; }
        public DbSet<Flashcard> Flashcards { get; set; }

        public RemoteMyDbContext()
            : base("name=RemoteMyDbContext")
        {
        }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //    if (modelBuilder == null)
            //    {
            //        throw new ArgumentNullException("modelBuilder");
            //    }

            //    base.OnModelCreating(modelBuilder);

            //    modelBuilder.Entity<TestModel>().Property(u => u.id).HasMaxLength(128);

            //    //Uncomment this to have Email length 128 too (not neccessary)
            //    //modelBuilder.Entity<ApplicationUser>().Property(u => u.Email).HasMaxLength(128);

            //    modelBuilder.Entity<IdentityRole>().Property(r => r.Name).HasMaxLength(128);
        }


    }
}
