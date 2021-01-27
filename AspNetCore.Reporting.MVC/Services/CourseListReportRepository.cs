using System;
using System.Collections.Generic;
using System.Linq;
using AspNetCore.Reporting.MVC.Data;
using AspNetCore.Reporting.Common.Models;

namespace AspNetCore.Reporting.Common.Services {
    public class CourseListReportRepository {
        readonly IScopedDbContextProvider<SchoolDbContext> scopedDbContextProvider;

        public CourseListReportRepository() {
            // We use this parameterless constructor in the Data Source Wizard only, and not for the actual instantiation of the repository object.
            throw new NotSupportedException();
        }

        public CourseListReportRepository(IScopedDbContextProvider<SchoolDbContext> scopedDbContextProvider) {
            this.scopedDbContextProvider = scopedDbContextProvider ?? throw new ArgumentNullException(nameof(scopedDbContextProvider));
        }

        public IList<CourseModel> GetCourses() {
            using(var dbContextScope = scopedDbContextProvider.GetDbContextScope()) {
                var dbContext = dbContextScope.DbContext;
                var model = dbContext.Courses.Select(x =>
                    new CourseModel {
                        CourseID = x.CourseID,
                        CourseTitle = x.Title
                    })
                    .ToList();
                return model;
            }
        }
    }
}
