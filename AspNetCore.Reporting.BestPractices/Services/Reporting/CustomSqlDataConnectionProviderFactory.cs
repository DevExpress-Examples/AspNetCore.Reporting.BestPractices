using DevExpress.DataAccess.Sql;
using DevExpress.DataAccess.Web;
using DevExpress.DataAccess.Wizard.Services;
using Microsoft.Extensions.Configuration;

namespace AspNetCoreReportingApp.Services.Reporting {
    public class CustomSqlDataConnectionProviderFactory : IConnectionProviderFactory {
        readonly IConnectionProviderService connectionProviderService;
        public CustomSqlDataConnectionProviderFactory(IConnectionProviderService connectionProviderService) {
            this.connectionProviderService = connectionProviderService;
        }
        public IConnectionProviderService Create() {
            return connectionProviderService;
        }
    }

    public class CustomConnectionProviderService : IConnectionProviderService {
        readonly IConfiguration configurationRoot;
        public CustomConnectionProviderService(IConfiguration configurationRoot) {
            this.configurationRoot = configurationRoot;
        }
        public SqlDataConnection LoadConnection(string connectionName) {
            var connections = configurationRoot.GetSection("ReportingConnectionStrings");
            var connectionString = connections.GetValue<string>(connectionName);
            return new SqlDataConnection { Name = connectionName, ConnectionString = connectionString };
        }
    }
}
