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
    public interface IAlertService
    {
        Task<AlertDto> CreateAlertAsync(AlertCreateDto alertCreateDto);
        Task<IList<AlertDto>> GetActiveAlertsAsync(int userId);
        Task<IList<AlertLogDto>> GetAlertsHistoryAsync(int userId);
        Task DeleteAlertAsync(int alertId);
    }
    public class AlertService : IAlertService
    {
        private readonly CryptoDbContext _context;
        private readonly IMapper _mapper;

        public AlertService(CryptoDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<AlertDto> CreateAlertAsync(AlertCreateDto alertCreateDto)
        {
            var crypto = await _context.Cryptos.FirstOrDefaultAsync(c => c.Id == alertCreateDto.CryptoId);
            if (crypto == null)
            {
                throw new Exception("Crypto not found");
            }
            var user = await _context.Users.FirstOrDefaultAsync(c => c.Id == alertCreateDto.UserId);
            if (user == null)
            {
                throw new Exception("User not found");
            }
            if (alertCreateDto.TargetPrice <= 0)
            {
                throw new Exception("Invalid TargetPrice");
            }
            if ((int)alertCreateDto.AlertType < 0 || (int)alertCreateDto.AlertType > 1)
            {
                throw new Exception("Invalid AlertType");
            }
            var alert = _mapper.Map<Alert>(alertCreateDto);
            await _context.Alerts.AddAsync(alert);

            await _context.SaveChangesAsync();

            var alertDto=_mapper.Map<AlertDto>(alert);
            return alertDto;
        }

        public async Task<IList<AlertDto>> GetActiveAlertsAsync(int userId)
        {
            var alerts = await _context.Alerts
                .Include(c => c.Crypto)
                .Where(a=> a.UserId==userId)
                .ToListAsync();
            return _mapper.Map<IList<AlertDto>>(alerts);
        }

        public async Task<IList<AlertLogDto>> GetAlertsHistoryAsync(int userId)
        {
            var alertlogs = await _context.AlertLogs
                .Include(c => c.Crypto)
                .Where(a => a.UserId == userId)
                .ToListAsync();
            return _mapper.Map<IList<AlertLogDto>>(alertlogs);
        }

        public async Task DeleteAlertAsync(int alertId)
        {
            var alert = await _context.Alerts.FirstOrDefaultAsync(c => c.Id == alertId);
            if(alert == default)
            {
                throw new Exception("Alert not found!");
            }
            _context.Alerts.Remove(alert);
            await _context.SaveChangesAsync();
        }
    }
}
