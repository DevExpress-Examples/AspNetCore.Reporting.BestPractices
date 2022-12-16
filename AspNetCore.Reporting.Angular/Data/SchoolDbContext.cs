using AspNetCore.Reporting.Common.Data;
using Duende.IdentityServer.EntityFramework.Options;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AspNetCore.Reporting.Angular.Data {
    public class SchoolDbContext : ApiAuthorizationDbContext<StudentIdentity>, IStudentEntityProvider, IReportEntityProvider {
        public SchoolDbContext(
            DbContextOptions<SchoolDbContext> options,
            IOptions<OperationalStoreOptions> operationalStoreOptions) : base(options, operationalStoreOptions) {
        }

        public DbSet<Course> Courses { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<StudentIdentity> Students { get; set; }
        public DbSet<ReportEntity> Reports { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Course>().ToTable("Course");
            modelBuilder.Entity<Enrollment>().ToTable("Enrollment");
            modelBuilder.Entity<StudentIdentity>().ToTable("Student");
            modelBuilder.Entity<ReportEntity>().ToTable("Report");
        }
    }
}
