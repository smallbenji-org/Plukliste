using Dapper;
using PluckFish.Models;
using Microsoft.AspNetCore.Identity;
using Npgsql;

namespace PluckFish.Auth
{
    public class UserStore : IUserStore<ApplicationUser>, IUserPasswordStore<ApplicationUser>
    {
        private readonly string _ConnectionString;

        public UserStore(IConfiguration config)
        {
            _ConnectionString = config.GetConnectionString("defaultConnection");
        }

        private NpgsqlConnection Connection => new NpgsqlConnection(_ConnectionString);

        public async Task<IdentityResult> CreateAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            var sql = "INSERT INTO users (id, username, normalized_username, password_hash, full_name) " +
                      "VALUES (@Id, @UserName, @NormalizedUserName, @PasswordHash, @FullName)";
            using NpgsqlConnection db = Connection;
            await db.ExecuteAsync(sql, user);
            return IdentityResult.Success;
        }

        public async Task<ApplicationUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            var sql = "SELECT * FROM users WHERE normalized_username = @NormalizedUserName";
            using NpgsqlConnection db = Connection;
            return await db.QueryFirstOrDefaultAsync<ApplicationUser>(sql, new { NormalizedUserName = normalizedUserName.ToUpper() });
        }

        public Task<string> GetUserIdAsync(ApplicationUser user, CancellationToken cancellationToken)
            => Task.FromResult(user.Id);

        public Task<string> GetUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
            => Task.FromResult(user.UserName);

        public Task SetUserNameAsync(ApplicationUser user, string userName, CancellationToken cancellationToken)
        {
            user.UserName = userName;
            return Task.CompletedTask;
        }

        public Task<string> GetNormalizedUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
            => Task.FromResult(user.NormalizedUserName);

        public Task SetNormalizedUserNameAsync(ApplicationUser user, string normalizedName, CancellationToken cancellationToken)
        {
            user.NormalizedUserName = normalizedName;
            return Task.CompletedTask;
        }

        public Task SetPasswordHashAsync(ApplicationUser user, string passwordHash, CancellationToken cancellationToken)
        {
            user.PasswordHash = passwordHash;
            return Task.CompletedTask;
        }

        public Task<string> GetPasswordHashAsync(ApplicationUser user, CancellationToken cancellationToken)
            => Task.FromResult(user.PasswordHash);

        public Task<bool> HasPasswordAsync(ApplicationUser user, CancellationToken cancellationToken)
            => Task.FromResult(!string.IsNullOrWhiteSpace(user.PasswordHash));

        public Task<IdentityResult> DeleteAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            var sql = "DELETE FROM users WHERE id = @Id";
            using NpgsqlConnection db = Connection;
            db.Execute(sql, new { user.Id });
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> UpdateAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            var sql = "UPDATE users SET username = @UserName, normalized_username = @NormalizedUserName, " +
                      "password_hash = @PasswordHash, full_name = @FullName WHERE id = @Id";
            using NpgsqlConnection db = Connection;
            db.Execute(sql, user);
            return Task.FromResult(IdentityResult.Success);
        }

        public async Task<ApplicationUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            var sql = "SELECT * FROM users WHERE id = @Id";
            using NpgsqlConnection db = Connection;
            return await db.QueryFirstOrDefaultAsync<ApplicationUser>(sql, new { Id = userId });
        }

        public void Dispose() { }
    }
}