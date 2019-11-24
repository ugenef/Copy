using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Billing.Models
{
    
    public class Call
    {
        [Key] public string CallId { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string CallingNumber { get; set; }
        public string CalledNumber { get; set; }
        public int Duration { get; set; }
        public CallType CallType { get; set; }
    }
}