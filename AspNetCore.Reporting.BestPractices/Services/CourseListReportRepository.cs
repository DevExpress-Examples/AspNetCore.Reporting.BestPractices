using System;
using System.Collections.Generic;
using System.Linq;
using AspNetCoreReportingApp.Data;
using AspNetCoreReportingApp.Models;

namespace AspNetCoreReportingApp.Services {
    public class CourseListReportRepository {
        readonly IScopedDbContextProvider<SchoolContext> scopedDbContextProvider;

        public CourseListReportRepository() {
            // We use this parameterless constructor in the Data Source Wizard only, and not for the actual instantiation of the repository object.
            throw new NotSupportedException();
        }

        public CourseListReportRepository(IScopedDbContextProvider<SchoolContext> scopedDbContextProvider) {
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
