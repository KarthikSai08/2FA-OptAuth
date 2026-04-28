using Application.Interfaces.Repositories;
using Dapper;
using Domain.Entities;
using Infrastructure.Persistence;
using System.Data;

namespace Infrastructure.Repositories
{
    public class UserRepository(DapperContext context) : IUserRepository
    {
        public async Task<int> CreateUserAsync(string email, string passwordHash, string userName)
        {
            using var con = context.CreateConection();
            return await con.ExecuteScalarAsync<int>(
                "sp_CreateUser",
                new { Email = email, PasswordHash = passwordHash, UserName = userName },
                commandType: CommandType.StoredProcedure);
        }
        public async Task<User?> GetByEmailAsync(string email)
        {
            using var con = context.CreateConection();
            return await con.QuerySingleOrDefaultAsync<User>(
                "sp_GetUserByEmail",
                new {Email = email},
                commandType: CommandType.StoredProcedure);
        }
    }
}
