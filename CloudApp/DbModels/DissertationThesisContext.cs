using Microsoft.EntityFrameworkCore;

namespace CloudApp.DbModels
{
    public partial class DissertationThesisContext : DbContext
    {
        public DissertationThesisContext()
        {
        }

        public DissertationThesisContext(DbContextOptions<DissertationThesisContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Users> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.3-servicing-35854");

            modelBuilder.Entity<Users>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasMaxLength(65)
                    .ValueGeneratedNever();

                entity.Property(e => e.PasswordHash)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.Role)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.User)
                    .IsRequired()
                    .HasMaxLength(256);
            });
        }
    }
}
