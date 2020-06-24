using System.Collections.Generic;
using System.Linq;
using DevExpress.Data.Entity;
using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.Web;
using Microsoft.Extensions.Configuration;

namespace AspNetCoreReportingApp.Services.Reporting {
    public class CustomSqlDataSourceWizardConnectionStringsProvider : IDataSourceWizardConnectionStringsProvider {
        IConfiguration Configuration { get; }
        public CustomSqlDataSourceWizardConnectionStringsProvider() : this(Startup.Configuration) { }
        public CustomSqlDataSourceWizardConnectionStringsProvider(IConfiguration configuration) {
            Configuration = configuration;
        }
        public Dictionary<string, string> GetConnectionDescriptions() {
            var connections = Configuration.GetSection("ReportingDataConnectionStrings").AsEnumerable(makePathsRelative: true).ToDictionary(x => x.Key, x => x.Key);
            return connections;
        }

        public DataConnectionParametersBase GetDataConnectionParameters(string name) {
            if(name != null)
                return null;//To prevent serialization connection parameters to the client
            //Skip lines below
            var connectionString = Configuration.GetSection("ReportingDataConnectionStrings").GetValue<string>(name);
            DataConnectionParametersBase connectionParameters;
            if(string.IsNullOrEmpty(connectionString) || !AppConfigHelper.TryCreateSqlConnectionParameters(new ConnectionStringInfo() { RunTimeConnectionString = connectionString, ProviderName= "SQLite" }, out connectionParameters)) {
                throw new KeyNotFoundException($"Connection string '{name}' not found.");
            }
            return connectionParameters;
        }
    }
}
