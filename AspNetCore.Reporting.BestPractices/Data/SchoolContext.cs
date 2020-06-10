using Microsoft.EntityFrameworkCore;

namespace AspNetCoreReportingApp.Data {
    public class SchoolContext : DbContext {
        public SchoolContext(DbContextOptions<SchoolContext> options) : base(options) {
        }

        public DbSet<Course> Courses { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Report> Reports { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<Course>().ToTable("Course");
            modelBuilder.Entity<Enrollment>().ToTable("Enrollment");
            modelBuilder.Entity<Student>().ToTable("Student");
            modelBuilder.Entity<Report>().ToTable("Report");
        }
    }
}
