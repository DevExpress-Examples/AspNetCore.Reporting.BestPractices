using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DevExpress.DataAccess.Web;

namespace AspNetCoreReportingApp.Services.Reporting {
    public class CustomObjectDataSourceWizardTypeProvider : IObjectDataSourceWizardTypeProvider {
        public IEnumerable<Type> GetAvailableTypes(string context) {
            return new[] {
                typeof(CourseListReportRepository),
                typeof(MyEnrollmentsReportRepository)
            };
        }
    }

    public class CustomObjectDataSourceConstructorFilterService : IObjectDataSourceConstructorFilterService {
        readonly IObjectDataSourceWizardTypeProvider wizardTypeProvider;

        public CustomObjectDataSourceConstructorFilterService(IObjectDataSourceWizardTypeProvider wizardTypeProvider) {
            this.wizardTypeProvider = wizardTypeProvider ?? throw new ArgumentNullException(nameof(wizardTypeProvider));
        }

        public IEnumerable<ConstructorInfo> Filter(Type dataSourceType, IEnumerable<ConstructorInfo> constructors) {
            if(wizardTypeProvider.GetAvailableTypes(null).Contains(dataSourceType)) {
                return constructors.Where(x => !x.GetParameters().Any());
            }

            return constructors;
        }
    }
}
