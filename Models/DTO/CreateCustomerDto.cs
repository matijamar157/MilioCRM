namespace AFuturaCRMV2.Models.DTO
{
    public class CreateCustomerDto
    {
        public string Name { get; set; }
        public string Country { get; set; }
        public string Owner { get; set; }
        public string TelephoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public int CustomerTypeId { get; set; }
        public int CustomerStatusId { get; set; }
        public decimal? DealValue { get; set; }
        public string Website { get; set; }
        public DateTime? DateOfLatestMeeting { get; set; }
        public bool PriorityLead { get; set; }
    }
}
