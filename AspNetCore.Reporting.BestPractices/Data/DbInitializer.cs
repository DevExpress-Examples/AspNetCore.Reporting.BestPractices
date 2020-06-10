using System;
using System.IO;
using System.Linq;
using DevExpress.XtraReports.UI;
using AspNetCoreReportingApp.Reports;

namespace AspNetCoreReportingApp.Data {
    public static class DbInitializer {
        public static void Initialize(SchoolContext context, ReportsFactory factory) {
            //context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            // Look for any students.
            if(context.Students.Any()) {
                return;   // DB has been seeded
            }

            var students = new Student[] {
                new Student{ FirstMidName = "Carson", LastName = "Alexander", EnrollmentDate = DateTime.Parse("2005-09-01") },
                new Student{ FirstMidName = "Meredith", LastName = "Alonso", EnrollmentDate = DateTime.Parse("2002-09-01") },
                new Student{ FirstMidName = "Arturo", LastName = "Anand", EnrollmentDate = DateTime.Parse("2003-09-01") },
                new Student{ FirstMidName = "Gytis", LastName = "Barzdukas", EnrollmentDate = DateTime.Parse("2002-09-01") },
                new Student{ FirstMidName = "Yan", LastName = "Li", EnrollmentDate = DateTime.Parse("2002-09-01") },
                new Student{ FirstMidName = "Peggy", LastName = "Justice", EnrollmentDate = DateTime.Parse("2001-09-01") },
                new Student{ FirstMidName = "Laura", LastName = "Norman", EnrollmentDate = DateTime.Parse("2003-09-01") },
                new Student{ FirstMidName = "Nino", LastName = "Olivetto", EnrollmentDate = DateTime.Parse("2005-09-01") }
            };
            foreach(Student s in students) {
                context.Students.Add(s);
                foreach(var report in factory.Reports.Select(a => new { a.Key, Value = a.Value() })) {
                    context.Reports.Add(new Report() {
                        DisplayName = string.IsNullOrEmpty(report.Value.DisplayName) ? report.Key : report.Value.DisplayName,
                        ReportLayout = ReportToByteArray(report.Value),
                        Student = s
                    });
                }
            }
            context.SaveChanges();
            var courses = new Course[] {
                new Course { CourseID = 1050, Title = "Chemistry", Credits =  3 },
                new Course { CourseID = 4022, Title = "Microeconomics", Credits =  3 },
                new Course { CourseID = 4041, Title = "Macroeconomics", Credits =  3 },
                new Course { CourseID = 1045, Title = "Calculus", Credits =  4 },
                new Course { CourseID = 3141, Title = "Trigonometry", Credits =  4 },
                new Course { CourseID = 2021, Title = "Composition", Credits =  3 },
                new Course { CourseID = 2042, Title = "Literature", Credits =  4 }
            };
            foreach(Course c in courses) {
                context.Courses.Add(c);
            }
            context.SaveChanges();

            var enrollments = new Enrollment[] {
                new Enrollment { Student = students[0], Course = courses[0], Grade = Grade.A },
                new Enrollment { Student = students[1], Course = courses[2], Grade = Grade.C },
                new Enrollment { Student = students[3], Course = courses[4], Grade = Grade.B },
                new Enrollment { Student = students[2], Course = courses[1], Grade = Grade.B },
                new Enrollment { Student = students[1], Course = courses[4], Grade = Grade.F },
                new Enrollment { Student = students[5], Course = courses[3], Grade = Grade.F },
                new Enrollment { Student = students[2], Course = courses[2] },
                new Enrollment { Student = students[1], Course = courses[1] },
                new Enrollment { Student = students[2], Course = courses[5], Grade = Grade.F },
                new Enrollment { Student = students[2], Course = courses[6], Grade = Grade.C },
                new Enrollment { Student = students[6], Course = courses[2] },
                new Enrollment { Student = students[0], Course = courses[0], Grade = Grade.A },
            };
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
