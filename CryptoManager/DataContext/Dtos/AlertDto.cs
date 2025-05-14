using DataContext.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataContext.Dtos
{
    public class AlertDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string CryptoName { get; set; }
        public string Symbol { get; set;}
        public int TargetPrice { get; set; }
        public string AlertType { get; set; }
    }

    public class AlertLogDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string CryptoName { get; set; }
        public string Symbol { get; set; }
        public int TargetPrice { get; set; }
        public string AlertType { get; set; }
        public DateTime AlertDateTime { get; set; }
    }

    public class AlertCreateDto
    {
        public int UserId { get; set; }
        public int CryptoId { get; set; }
        public int TargetPrice { get; set; }
        public EAlertType AlertType { get; set; }
    }
}
