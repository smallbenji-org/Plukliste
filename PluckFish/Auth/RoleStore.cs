using Dapper;
using PluckFish.Models;
using Microsoft.AspNetCore.Identity;
using Npgsql;
using System.Data;

namespace PluckFish.Auth
{
    public class RoleStore : IRoleStore<ApplicationRole>
    {
        private readonly string _connectionString;

        public RoleStore(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("defaultConnection");
        }

        private IDbConnection Connection => new NpgsqlConnection(_connectionString);

        public async Task<IdentityResult> CreateAsync(ApplicationRole role, CancellationToken cancellationToken)
        {
            const string sql = @"
            INSERT INTO roles (id, name, normalized_name, description)
            VALUES (@Id, @Name, @NormalizedName, @Description)";
            using IDbConnection db = Connection;
            await db.ExecuteAsync(sql, role);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> UpdateAsync(ApplicationRole role, CancellationToken cancellationToken)
        {
            const string sql = @"
            UPDATE roles
            SET name = @Name,
                normalized_name = @NormalizedName,
                description = @Description
            WHERE id = @Id";
            using IDbConnection db = Connection;
            await db.ExecuteAsync(sql, role);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(ApplicationRole role, CancellationToken cancellationToken)
        {
            const string sql = "DELETE FROM roles WHERE id = @Id";
            using IDbConnection db = Connection;
            await db.ExecuteAsync(sql, new { role.Id });
            return IdentityResult.Success;
        }

        public async Task<ApplicationRole> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            const string sql = "SELECT * FROM roles WHERE id = @Id";
            using IDbConnection db = Connection;
            return await db.QuerySingleOrDefaultAsync<ApplicationRole>(sql, new { Id = roleId });
        }

        public async Task<ApplicationRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            const string sql = "SELECT * FROM roles WHERE normalized_name = @NormalizedName";
            using IDbConnection db = Connection;
            return await db.QuerySingleOrDefaultAsync<ApplicationRole>(sql, new { NormalizedName = normalizedRoleName });
        }

        public Task<string> GetRoleIdAsync(ApplicationRole role, CancellationToken cancellationToken)
            => Task.FromResult(role.Id);

        public Task<string> GetRoleNameAsync(ApplicationRole role, CancellationToken cancellationToken)
            => Task.FromResult(role.Name);

        public Task SetRoleNameAsync(ApplicationRole role, string roleName, CancellationToken cancellationToken)
        {
            role.Name = roleName;
            return Task.CompletedTask;
        }

        public Task<string> GetNormalizedRoleNameAsync(ApplicationRole role, CancellationToken cancellationToken)
            => Task.FromResult(role.NormalizedName);

        public Task SetNormalizedRoleNameAsync(ApplicationRole role, string normalizedName, CancellationToken cancellationToken)
        {
            role.NormalizedName = normalizedName;
            return Task.CompletedTask;
        }

        public void Dispose() { }
    }
}