namespace UGTS.Models
{
    public class ObjectionNoficationModel
    {
        public string ObjectionId { get; set; } = string.Empty;
        public string UgtsReferenceNo { get; set; } = string.Empty;
        public string TransactionReferenceNo { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? TelephoneNumber { get; set; }
        public string Plate { get; set; } = string.Empty;
        public string ObjectionStatus { get; set; } = string.Empty;
        public string ObjectionDate { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
