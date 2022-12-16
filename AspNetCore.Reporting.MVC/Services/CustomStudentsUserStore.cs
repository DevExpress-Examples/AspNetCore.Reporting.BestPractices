using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AspNetCore.Reporting.Common.Data;
using AspNetCore.Reporting.MVC.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AspNetCore.Reporting.Common.Services {
    /// <summary>
    /// This store is only partially implemented. It supports user creation and find methods.
    /// </summary>
    public class CustomStudentsUserStore : IUserEmailStore<StudentIdentity>,
        IUserPasswordStore<StudentIdentity> {
        private readonly SchoolDbContext dbContext;

        public CustomStudentsUserStore(SchoolDbContext dbContext) {
            this.dbContext = dbContext;
        }

        public async Task<IdentityResult> CreateAsync(StudentIdentity user,
            CancellationToken cancellationToken = default(CancellationToken)) {
            cancellationToken.ThrowIfCancellationRequested();
            if(user == null) throw new ArgumentNullException(nameof(user));

            await dbContext.Users.AddAsync(user, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(StudentIdentity user,
            CancellationToken cancellationToken = default(CancellationToken)) {
            cancellationToken.ThrowIfCancellationRequested();
            if(user == null) throw new ArgumentNullException(nameof(user));
            dbContext.Users.Remove(user);
            await dbContext.SaveChangesAsync(cancellationToken);
            return IdentityResult.Success;

        }

        public void Dispose() {
        }

        public Task<StudentIdentity> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        public async Task<StudentIdentity> FindByIdAsync(string userId,
            CancellationToken cancellationToken = default(CancellationToken)) {
            cancellationToken.ThrowIfCancellationRequested();
            if(userId == null) throw new ArgumentNullException(nameof(userId));
            Guid idGuid;
            if(!Guid.TryParse(userId, out idGuid)) {
                throw new ArgumentException("Not a valid Guid id", nameof(userId));
            }

            return await dbContext.Users.Where(x => x.Id == idGuid.ToString("N")).FirstOrDefaultAsync();

        }

        public async Task<StudentIdentity> FindByNameAsync(string userName,
            CancellationToken cancellationToken = default(CancellationToken)) {
            cancellationToken.ThrowIfCancellationRequested();
            if(userName == null) throw new ArgumentNullException(nameof(userName));

            return await dbContext.Users.Where(x => x.NormalizedUserName == userName).FirstOrDefaultAsync();
        }

        public async Task<string> GetEmailAsync(StudentIdentity user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            if(user == null) throw new ArgumentNullException(nameof(user));

            var exitedUser = await dbContext.Users.Where(x => x.NormalizedUserName == user.NormalizedUserName).FirstOrDefaultAsync();
            return exitedUser.Email;
        }

        public async Task<bool> GetEmailConfirmedAsync(StudentIdentity user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            if(user == null) throw new ArgumentNullException(nameof(user));

            var exitedUser = await dbContext.Users.Where(x => x.NormalizedUserName == user.NormalizedUserName).FirstOrDefaultAsync();
            return exitedUser.EmailConfirmed;
        }

        public async Task<string> GetNormalizedEmailAsync(StudentIdentity user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            if(user == null) throw new ArgumentNullException(nameof(user));

            var exitedUser = await dbContext.Users.Where(x => x.NormalizedUserName == user.NormalizedUserName).FirstOrDefaultAsync();
            return exitedUser.NormalizedEmail;
        }

        public Task<string> GetNormalizedUserNameAsync(StudentIdentity user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            if(user == null) throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.NormalizedUserName);
        }

        public Task<string> GetPasswordHashAsync(StudentIdentity user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            if(user == null) throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.PasswordHash);
        }

        public Task<string> GetUserIdAsync(StudentIdentity user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            if(user == null) throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.Id.ToString());
        }

        public Task<string> GetUserNameAsync(StudentIdentity user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            if(user == null) throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.UserName);
        }

        public Task<bool> HasPasswordAsync(StudentIdentity user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            if(user == null) throw new ArgumentNullException(nameof(user));

            return Task.FromResult(!string.IsNullOrEmpty(user.PasswordHash));
        }

        public Task SetEmailAsync(StudentIdentity user, string email, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        public Task SetEmailConfirmedAsync(StudentIdentity user, bool confirmed, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        public Task SetNormalizedEmailAsync(StudentIdentity user, string normalizedEmail, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        public Task SetNormalizedUserNameAsync(StudentIdentity user, string normalizedName, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            if(user == null) throw new ArgumentNullException(nameof(user));
            if(normalizedName == null) throw new ArgumentNullException(nameof(normalizedName));

            user.NormalizedUserName = normalizedName;
            return Task.FromResult<object>(null);
        }

        public Task SetPasswordHashAsync(StudentIdentity user, string passwordHash, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            if(user == null) throw new ArgumentNullException(nameof(user));
            if(passwordHash == null) throw new ArgumentNullException(nameof(passwordHash));

            user.PasswordHash = passwordHash;
            return Task.FromResult<object>(null);

        }

        public Task SetUserNameAsync(StudentIdentity user, string userName, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> UpdateAsync(StudentIdentity user, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }
    }
}
