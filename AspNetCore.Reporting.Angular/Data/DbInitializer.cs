using System.IO;
using System.Linq;
using AspNetCore.Reporting.Common.Data;
using AspNetCore.Reporting.Common.Reports;
using DevExpress.XtraReports.UI;
using Microsoft.AspNetCore.Identity;

namespace AspNetCore.Reporting.Angular.Data {
    public static class DbInitializer {
        public static void Initialize(SchoolDbContext context, UserManager<StudentIdentity> userManager, ReportsFactory factory) {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            // Look for any students.
            if(context.Students.Any()) {
                return;   // DB has been seeded
            }

            var students = DbDefaultsGenerator.GenerateStudents(userManager);
            foreach(StudentIdentity s in students) {

                context.Students.Add(s);
                foreach(var report in factory.Reports.Select(a => new { a.Key, Value = a.Value() })) {
                    context.Reports.Add(new ReportEntity() {
                        DisplayName = string.IsNullOrEmpty(report.Value.DisplayName) ? report.Key : report.Value.DisplayName,
                        ReportLayout = ReportToByteArray(report.Value),
                        Student = s
                    });
                }
            }
            context.SaveChanges();
            var courses = DbDefaultsGenerator.GetCourses();
            foreach(Course c in courses) {
                context.Courses.Add(c);
            }
            context.SaveChanges();

            var enrollments = DbDefaultsGenerator.GetEnrollments(students, courses);
            foreach(Enrollment e in enrollments) {
                context.Enrollments.Add(e);
            }
            context.SaveChanges();
        }

        static byte[] ReportToByteArray(XtraReport report) {
            using(var memoryStream = new MemoryStream()) {
                report.SaveLayoutToXml(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }
}
