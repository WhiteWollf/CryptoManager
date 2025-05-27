using DataContext.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataContext.Dtos
{
    public class SavingLockCreateDto
    {
        public int UserId { get; set; }
        public int CryptoId { get; set; }
        public decimal Amount { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class SavingLockDto
    {
        public int Id { get; set; }
        public string CryptoName { get; set; }
        public decimal Amount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal InterestRate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
    }

    public class UserSavingLockDto
    {
        public List<SavingLockDto> ActiveLocks { get; set; }
        public List<SavingLockDto> ExpiredLocks { get; set; }
    }
}
