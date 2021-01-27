using System;
using DevExpress.DataAccess.ObjectBinding;
using DevExpress.XtraReports.Native.Data;
using DevExpress.XtraReports.UI;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Reporting.Common.Services {
    public interface IObjectDataSourceInjector {
        void Process(XtraReport report);
    }

    class ObjectDataSourceInjector : IObjectDataSourceInjector {
        IServiceProvider ServiceProvider { get; }

        public ObjectDataSourceInjector(IServiceProvider serviceProvider) {
            ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public void Process(XtraReport report) {
            var dse = new UniqueDataSourceEnumerator();
            foreach(var dataSource in dse.EnumerateDataSources(report, true)) {
                if(dataSource is ObjectDataSource ods && ods.DataSource is Type dataSourceType) {
                    ods.DataSource = ServiceProvider.GetRequiredService(dataSourceType);
                }
            }
        }
    }
}
