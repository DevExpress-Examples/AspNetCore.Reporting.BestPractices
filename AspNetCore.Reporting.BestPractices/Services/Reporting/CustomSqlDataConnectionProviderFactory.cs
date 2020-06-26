using System;
using System.Collections.Generic;
using DevExpress.DataAccess.ConnectionParameters;
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
            var connections = configurationRoot.GetSection("ReportingDataConnectionStrings");
            var connectionString = connections.GetValue<string>(connectionName);
            if(string.IsNullOrEmpty(connectionString))
                throw new ArgumentException("There is no connection with name: " + connectionName);
            return new SqlDataConnection { 
                Name = connectionName, 
                ConnectionString = connectionString, 
                StoreConnectionNameOnly = true, 
                ConnectionStringSerializable = connectionString, 
                ProviderKey = "SQLite" 
            };
        }

        public SqlDataConnection LoadConnection2(string connectionName) {
            var connections = configurationRoot.GetSection("ReportingDataConnectionStrings");
            var connectionString = connections.GetValue<string>(connectionName);
            DataConnectionParametersBase connectionParameters;
            if(string.IsNullOrEmpty(connectionString) || !DevExpress.DataAccess.Native.AppConfigHelper.TryCreateSqlConnectionParameters(new DevExpress.Data.Entity.ConnectionStringInfo() { RunTimeConnectionString = connectionString, ProviderName = "SQLite" }, out connectionParameters)) {
                throw new KeyNotFoundException($"Connection string '{connectionName}' not found.");
            }
            if(connectionParameters != null) {
                return new SqlDataConnection(connectionName, connectionParameters);
            }
            throw new ArgumentException("There is no connection with name: " + connectionName);
            //return new SqlDataConnection { Name = connectionName, ConnectionString = connectionString, StoreConnectionNameOnly = true };

            
            //DataConnectionParametersBase connectionParameters;
            //if(string.IsNullOrEmpty(connectionString) || !AppConfigHelper.TryCreateSqlConnectionParameters(new ConnectionStringInfo() { RunTimeConnectionString = connectionString, ProviderName = "SQLite" }, out connectionParameters)) {
            //    throw new KeyNotFoundException($"Connection string '{name}' not found.");
            //}
            //return connectionParameters;


            //var parameters = DevExpress.DataAccess.Native.Web.DataSourceSerializationService.TryToGetDataConnectionParameters(wizardConnectionProvider, connectionName);
            //if(parameters != null) {
            //    return new SqlDataConnection(connectionName, parameters);
            //}
        }
    }
}
