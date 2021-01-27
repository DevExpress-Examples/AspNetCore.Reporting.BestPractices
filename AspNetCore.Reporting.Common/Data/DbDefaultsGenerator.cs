using System;
using Microsoft.AspNetCore.Identity;

namespace AspNetCore.Reporting.Common.Data {
    public static class DbDefaultsGenerator {

        public static StudentIdentity[] GenerateStudents(UserManager<StudentIdentity> userManager) {
            var students = new StudentIdentity[] {
                new StudentIdentity{ FirstMidName = "Carson", LastName = "Alexander", EnrollmentDate = DateTime.Parse("2005-09-01") },
                new StudentIdentity{ FirstMidName = "Meredith", LastName = "Alonso", EnrollmentDate = DateTime.Parse("2002-09-01") },
                new StudentIdentity{ FirstMidName = "Arturo", LastName = "Anand", EnrollmentDate = DateTime.Parse("2003-09-01") },
                new StudentIdentity{ FirstMidName = "Gytis", LastName = "Barzdukas", EnrollmentDate = DateTime.Parse("2002-09-01") },
                new StudentIdentity{ FirstMidName = "Yan", LastName = "Li", EnrollmentDate = DateTime.Parse("2002-09-01") },
                new StudentIdentity{ FirstMidName = "Peggy", LastName = "Justice", EnrollmentDate = DateTime.Parse("2001-09-01") },
                new StudentIdentity{ FirstMidName = "Laura", LastName = "Norman", EnrollmentDate = DateTime.Parse("2003-09-01") },
                new StudentIdentity{ FirstMidName = "Nino", LastName = "Olivetto", EnrollmentDate = DateTime.Parse("2005-09-01") }
            };
            foreach(var student in students) {
                student.UserName = student.FirstMidName + student.LastName + "@sample.email";
                student.NormalizedUserName = userManager?.NormalizeName(student.UserName);
                student.Email = student.UserName;
                student.NormalizedEmail = userManager?.NormalizeEmail(student.Email);
                student.EmailConfirmed = true;
                student.LockoutEnabled = true;
            }
            return students;
        }

        public static Course[] GetCourses() {
            return new Course[] {
                new Course { CourseID = 1050, Title = "Chemistry", Credits =  3 },
                new Course { CourseID = 4022, Title = "Microeconomics", Credits =  3 },
                new Course { CourseID = 4041, Title = "Macroeconomics", Credits =  3 },
                new Course { CourseID = 1045, Title = "Calculus", Credits =  4 },
                new Course { CourseID = 3141, Title = "Trigonometry", Credits =  4 },
                new Course { CourseID = 2021, Title = "Composition", Credits =  3 },
                new Course { CourseID = 2042, Title = "Literature", Credits =  4 }
            };
        }

        public static Enrollment[] GetEnrollments(StudentIdentity[] students, Course[] courses) {
            return new Enrollment[] {
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
        }
    }
}
