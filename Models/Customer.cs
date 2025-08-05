using System.ComponentModel.DataAnnotations;
using System.Transactions;

namespace AFuturaCRMV2.Models
{
    public class Customer
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
        public string Owner { get; set; }
        public string? TelephoneNumber { get; set; } = "No telephone";
        public string? EmailAddress { get; set; } = "No number";
        public int StatusId { get; set; }
        public int CustomerTypeId { get; set; }
        public CustomerType CustomerType { get; set; }
        public int CustomerStatusId { get; set; }
        public CustomerStatus CustomerStatus { get; set; }
        public int DealValue { get; set; } = 0;
        public DateTime DateOfLastInteraction = DateTime.Now;
        public string Website { get; set; }
        public DateTime DateOfLatestMeeting { get; set; }
        public bool PriorityLead { get; set; }
        public int CompanyId { get; set; }
        public Company Company { get; set; }

    }
}
