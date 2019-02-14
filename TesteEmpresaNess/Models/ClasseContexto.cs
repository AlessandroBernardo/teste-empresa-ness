using Microsoft.EntityFrameworkCore;

namespace TesteNess.Models
{
    public class ClasseContexto : DbContext
    {
        public ClasseContexto(DbContextOptions<ClasseContexto> options) : base(options)
        {


        }

        public DbSet<User> Users { get; set; }

        //private void ConfigUser(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<User>(user =>
        //    {
        //        user.Property(x => x.Latitude).HasColumnType("decimal(10,8)");
        //        user.Property(x => x.Longitude).HasColumnType("decimal(14,4)");
        //    });
        //}

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.ForSqlServerUseIdentityColumns();
        //}

    }
}
