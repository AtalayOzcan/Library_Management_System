using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Models
{
    public class Context : DbContext
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<Category> Categorys { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<Loan> Loans { get; set; }
        public DbSet<Publisher> Publishers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Book>(entity =>
            {
                entity.HasKey(b => b.BookId);

                entity.Property(b => b.ISBN)
                .HasMaxLength(13)
                .IsRequired();

                entity.HasIndex(b => b.ISBN)
                .IsUnique();

                entity.Property(b => b.Title)
                .HasMaxLength(200)
                .IsRequired();

                entity.Property(b => b.Author)
                .HasMaxLength(100)
                .IsRequired();

                entity.Property(b => b.Description)
                .HasMaxLength(1000);

                entity.HasOne(b => b.Category)       // 1. Bir kitabın BİR kategorisi vardır.
                .WithMany(c => c.Books)        // 2. O kategorinin ÇOK kitabı olabilir.
                .HasForeignKey(b => b.CategoryId); // 3. Bu bağ, 'CategoryId' sütunu ile sağlanır.

            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(c => c.CategoryId);

                entity.Property(c => c.CategoryName)
                .HasMaxLength(100)
                .IsRequired();

                entity.Property (c => c.Description)
                .HasMaxLength(500);
            });

            modelBuilder.Entity<Member>(entity =>
            {
                entity.HasKey(m => m.MemberId);

                entity.Property(m => m.MemberNumber)
                .HasMaxLength(20)
                .IsRequired();

                entity.HasIndex(m => m.MemberNumber)
                .IsUnique();

                entity.Property(m => m.FirstName)
                .HasMaxLength(50)
                .IsRequired();

                entity.Property(m => m.LastName)
                .HasMaxLength(50)
                .IsRequired();

                entity.Property(m => m.Email)
                .HasMaxLength(100)
                .IsRequired();

                entity.HasIndex (m => m.Email)
                .IsUnique();

                entity.Property(m => m.Phone)
                .HasMaxLength(11);

                entity.Property(m => m.Address)
                .HasMaxLength(500);
            });

            modelBuilder.Entity<Loan>(entity =>
            {
                entity.HasKey(l => l.LoanId);

                entity.Property(l => l.Fine).HasColumnType("decimal(18,2)");

                entity.HasOne(l => l.Book)
                .WithMany()
                .HasForeignKey(l => l.BookId);

                entity.HasOne(l => l.Member)
                .WithMany(m => m.Loans)
                .HasForeignKey (l => l.MemberId);
            });

            modelBuilder.Entity<Publisher>(entity =>
            {
                entity.HasKey(p => p.PublisherId);

                entity.Property(p => p.PublisherName)
                .HasMaxLength(100);

                entity.HasMany(p => p.Books)
                .WithOne(b => b.Publisher)
                .HasForeignKey(b => b.PublisherId);
            });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Database=LibraryManagementSystemDB;Username=postgres;Password=12345");
        }
    }
}
