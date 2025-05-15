using Microsoft.EntityFrameworkCore;
using StudentSystem.Models;

namespace StudentSystem.Data
{
    public class StudentSystemContext : DbContext
    {
        public StudentSystemContext()
        {
        }

        public StudentSystemContext(DbContextOptions<StudentSystemContext> options)
            : base(options)
        {
        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<Homework> HomeworkSubmissions { get; set; }
        public DbSet<StudentCourse> StudentCourses { get; set; }
        public DbSet<License> Licenses { get; set; }
        public DbSet<ResourceLicense> ResourceLicenses { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Data Source=StudentSystem.db");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure StudentCourse composite key
            modelBuilder.Entity<StudentCourse>()
                .HasKey(sc => new { sc.StudentId, sc.CourseId });

            // Configure StudentCourse relationships
            modelBuilder.Entity<StudentCourse>()
                .HasOne(sc => sc.Student)
                .WithMany(s => s.CourseEnrollments)
                .HasForeignKey(sc => sc.StudentId);

            modelBuilder.Entity<StudentCourse>()
                .HasOne(sc => sc.Course)
                .WithMany(c => c.StudentsEnrolled)
                .HasForeignKey(sc => sc.CourseId);

            modelBuilder.Entity<ResourceLicense>()
                .HasKey(rl => new { rl.ResourceId, rl.LicenseId });

            modelBuilder.Entity<ResourceLicense>()
                .HasOne(rl => rl.Resource)
                .WithMany(r => r.Licenses)
                .HasForeignKey(rl => rl.ResourceId);

            modelBuilder.Entity<ResourceLicense>()
                .HasOne(rl => rl.License)
                .WithMany(l => l.Resources)
                .HasForeignKey(rl => rl.LicenseId);
        }
    }
}