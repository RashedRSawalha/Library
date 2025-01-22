
using LibraryManagementDomain.Entities;
using Microsoft.EntityFrameworkCore;


namespace LibraryManagementInfrastructure
{
    public class LibraryDBContext : DbContext
    {
        public DbSet<Student> Students { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
      

        public LibraryDBContext(DbContextOptions<LibraryDBContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Define the many-to-many relationship between Students and Courses
            modelBuilder.Entity<Student>()
                .HasMany(s => s.Courses)
                .WithMany(c => c.Students)
                .UsingEntity<Dictionary<string, object>>(
                    "StudentCourse", // Name of the join table
                    j => j.HasOne<Course>().WithMany().HasForeignKey("CourseId"), // Course FK
                    j => j.HasOne<Student>().WithMany().HasForeignKey("StudentId") // Student FK
                );

            // Configure the Author-Book relationship
            modelBuilder.Entity<Book>()
                .HasOne(b => b.Author)
                .WithMany(a => a.Books)
                .HasForeignKey(b => b.AuthorId);

            // Configure Author entity to use enum
            modelBuilder.Entity<Author>()
                .Property(a => a.AuthorType)
                .HasConversion<int>(); // Store enum as integer in the database

            // Additional configurations (optional)
            modelBuilder.Entity<Student>()
                .Property(s => s.StudentName)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Course>()
                .Property(c => c.Title)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Author>()
                .Property(a => a.Name)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Book>()
                .Property(b => b.Title)
                .IsRequired()
                .HasMaxLength(200);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id); // Primary Key
                entity.Property(u => u.PasswordHash).IsRequired();
                entity.Property(u => u.RoleId).IsRequired();
            });

            // Table configuration for Roles
            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(r => r.Id); // Primary Key
                entity.Property(r => r.Name).IsRequired();
            });

            // Table configuration for RolePermissions
            modelBuilder.Entity<RolePermission>(entity =>
            {
                entity.HasKey(rp => rp.Id); // Primary Key
                entity.Property(rp => rp.RoleId).IsRequired(); // Foreign Key reference (mapped manually in the controller)
                entity.Property(rp => rp.Permission).IsRequired();
            });

            
        }
    }
}
