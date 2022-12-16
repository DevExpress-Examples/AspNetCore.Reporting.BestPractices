using AspNetCore.Reporting.Common.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AspNetCore.Reporting.MVC.Data {
    public class SchoolDbContext : IdentityDbContext<StudentIdentity>, IStudentEntityProvider, IReportEntityProvider {
        public SchoolDbContext(DbContextOptions<SchoolDbContext> options) : base(options) {
        }

        public DbSet<Course> Courses { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<StudentIdentity> Students { get; set; }
        public DbSet<ReportEntity> Reports { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Course>().ToTable("Courses");
            modelBuilder.Entity<Enrollment>().ToTable("Enrollments");
            modelBuilder.Entity<StudentIdentity>().ToTable("Students");
            modelBuilder.Entity<ReportEntity>().ToTable("Reports");
        }
    }
}
