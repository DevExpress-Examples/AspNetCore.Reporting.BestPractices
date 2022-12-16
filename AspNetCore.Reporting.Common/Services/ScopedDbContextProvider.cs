using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Reporting.Common.Services {
    public interface IScopedDbContextProvider<T> where T : DbContext {
        DbContextScope<T> GetDbContextScope();
    }

    // NOTE: This provider isolates the rest of the code from the IServiceProvider.
    // That way, we can clearly understand that the consumers of IScopedDbContextProvider requre specific scopes...
    public class ScopedDbContextProvider<T> : IScopedDbContextProvider<T> where T : DbContext {
        readonly IServiceProvider provider;

        public ScopedDbContextProvider(IServiceProvider provider) {
            this.provider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        public DbContextScope<T> GetDbContextScope() {
            var scope = provider.CreateScope();
            return new DbContextScope<T>(scope);
        }
    }

    public class DbContextScope<T> : IDisposable where T : DbContext {
        readonly IServiceScope scope;
        public T DbContext { get; private set; }

        public DbContextScope(IServiceScope scope) {
            this.scope = scope ?? throw new ArgumentNullException(nameof(scope));
            DbContext = scope.ServiceProvider.GetRequiredService<T>();
        }

        public void Dispose() {
            scope.Dispose();
        }
    }
}
