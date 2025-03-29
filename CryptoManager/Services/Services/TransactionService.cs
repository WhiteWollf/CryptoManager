﻿using AutoMapper;
using DataContext.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    public interface ITransactionService
    {
        Task<TransactionDto> GetTransactionsAsync(int userId);
        Task<DetailedTransactionDto> GetTransactionDetailsAsync(int transactionId);
    }
    public class TransactionService : ITransactionService
    {
        private readonly CryptoDbContext _context;
        private readonly IMapper _mapper;
        public TransactionService(CryptoDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<TransactionDto> GetTransactionsAsync(int userId)
        {
            var transactions = await _context.TransactionLogs
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.Timestamp)
                .Include(t => t.Crypto)
                .ToListAsync();
            return _mapper.Map<TransactionDto>(transactions);
        }
        public async Task<DetailedTransactionDto> GetTransactionDetailsAsync(int transactionId)
        {
            var transaction = await _context.TransactionLogs
                .Include(t => t.Crypto)
                .FirstOrDefaultAsync(t => t.Id == transactionId);
            if(transaction == null)
            {
                throw new Exception("Transaction not found");
            }
            return _mapper.Map<DetailedTransactionDto>(transaction);
        }
    }
}
