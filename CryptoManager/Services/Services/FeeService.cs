using AutoMapper;
using DataContext.Context;
using DataContext.Dtos;
using DataContext.Entities;
using DataContext.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    public interface IFeeService
    {
        Task<IList<TransactionFeeDto>> GetTransactionFeesAsync();
        Task<FeeSummaryDto> GetUserTransactionFeesAsync(int userId);
        Task<decimal> GetCurrentFeeAsync();
        Task ChangeFeeAsync(double newFee);
    }
    public class FeeService : IFeeService
    {
        private readonly CryptoDbContext _context;
        private readonly IMapper _mapper;

        public FeeService(CryptoDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<decimal> GetCurrentFeeAsync()
        {
            var fee = await _context.TransactionFee.Select(f=> f.Fee).FirstOrDefaultAsync();
            if (fee == default)
            {
                throw new Exception("Fee not found");
            }
            return fee;
        }

        public async Task ChangeFeeAsync(double newFee)
        {
            if (newFee < 0 || newFee > 50)
            {
                throw new Exception("The fee must be a value between 0% and 50%");
            }
            var transactionFee = await _context.TransactionFee.FirstOrDefaultAsync();
            transactionFee.Fee = (decimal)newFee;
            await _context.SaveChangesAsync();
        }

        public async Task<IList<TransactionFeeDto>> GetTransactionFeesAsync()
        {
            var transactions = await _context.TransactionLogs
                .Include(t => t.Crypto)
                .ToListAsync();
            if (transactions == null)
            {
                throw new Exception("Transactions not found");
            }
            return _mapper.Map<IList<TransactionFeeDto>>(transactions);
        }

        public async Task<FeeSummaryDto> GetUserTransactionFeesAsync(int userId)
        {
            var transactions = await _context.TransactionLogs
                .Where(t => t.UserId == userId)
                .Include(t => t.Crypto)
                .OrderByDescending(t => t.Timestamp)
                .ToListAsync();

            var transactionDtos = transactions.Select(t => new TransactionWithFeeDto
            {
                TransactionId = t.Id,
                CryptoId = t.CryptoId,
                CryptoName = t.Crypto?.Name ?? "N/A",
                FeePrice = t.FeePrice,
                BasePrice = t.BasePrice,
                TotalPrice = t.TotalPrice,
                Timestamp = t.Timestamp,
                Description = t.Description ?? "",
                Type = t.Type.ToString()
            }).ToList();

            var dailySummaries = transactionDtos
                .GroupBy(t => t.Timestamp.Date)
                .Select(g => new DailyFeeSummaryDto
                {
                    Date = g.Key,
                    DailyTotalFee = g.Sum(t => t.FeePrice),
                    Transactions = g.ToList()
                })
                .OrderByDescending(d => d.Date)
                .ToList();

            return new FeeSummaryDto
            {
                UserId = userId,
                TotalFeeAmount = transactionDtos.Sum(t => t.FeePrice),
                DailySummaries = dailySummaries
            };
        }
    }
}
