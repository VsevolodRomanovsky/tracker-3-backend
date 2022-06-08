using Microsoft.EntityFrameworkCore;

namespace Tracker.AuthenticationService.Data.Models
{
    public partial class TestDataContext : DbContext
    {
        public TestDataContext(DbContextOptions<TestDataContext> options)
            : base(options)
        {
        }

        public virtual DbSet<WebSecurityUsersExtended> WebSecurityUsersExtendeds { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WebSecurityUsersExtended>(entity =>
            {
                entity.ToTable("WebSecurityUsers_Extended");

                entity.HasKey(e => e.Userid);

                entity.Property(e => e.HasNewAccount)
                    .HasColumnName("HAS_NEW_ACCOUNT")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Userid)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("USERID")
                    .IsFixedLength();
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
