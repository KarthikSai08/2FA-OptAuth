using Application.Interfaces.Repositories;
using Dapper;
using Domain.Entities;
using Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Infrastructure.Repositories
{
    public class OtpRepository(DapperContext context) : IOtpRepository
    {
        public async Task<OtpRecord?> GetLatestOtpAsync(int userId)
        {
            using var con = context.CreateConection();
            return await con.QueryFirstOrDefaultAsync<OtpRecord>(
                "sp_GetLatestOtp",
                new { UserId = userId },
                commandType: CommandType.StoredProcedure);
        }

        public async Task InsertOtpAsync(int userId, string code, DateTime expiresAt)
        {
            using var con = context.CreateConection();
            await con.ExecuteAsync(
                "sp_InsertOtp",
                new
                {
                    UserId = userId,
                    Code = code,
                    ExpiresAt = expiresAt
                },
                commandType: CommandType.StoredProcedure);
        }

        public async Task MarkOtpUsedAsync(int otpId)
        {
            using var con = context.CreateConection();
            await con.ExecuteAsync(
                "sp_MarkOtpUsed",
                new {OtpId = otpId},
                commandType: CommandType.StoredProcedure);
        }
    }
}
