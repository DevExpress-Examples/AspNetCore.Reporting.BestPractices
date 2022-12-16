using System;
using System.Collections.Generic;
using DevExpress.Data.Entity;
using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.Sql;
using DevExpress.DataAccess.Web;
using DevExpress.DataAccess.Wizard.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace AspNetCore.Reporting.Common.Services.Reporting {
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
        readonly IConfiguration configuration;
        readonly IHttpContextAccessor httpContextAccessor;
        public CustomConnectionProviderService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor) {
            this.configuration = configuration;
            this.httpContextAccessor = httpContextAccessor;
        }

        public SqlDataConnection LoadConnection(string connectionName) {
            var connectionStringSection = configuration.GetSection("ReportingDataConnectionStrings");
            var connectionString = connectionStringSection?[connectionName];
            if (string.IsNullOrEmpty(connectionString))
                throw new KeyNotFoundException($"Connection string '{connectionName}' not found.");
            var connectionParameters = new CustomStringConnectionParameters(connectionString);
            return new SqlDataConnection(connectionName, connectionParameters);
        }

        public SqlDataConnection WrongLoadConnection(string connectionName) {
            var connectionString = configuration.GetSection("ReportingDataConnectionStrings")?[connectionName];
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
    }
}
