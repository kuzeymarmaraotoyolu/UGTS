using System.ComponentModel.DataAnnotations;

namespace UGTS.Models
{
    public class ObjectionNoficationModel
    {
        [Required(ErrorMessage = "ObjectionId is required.")]
        [RegularExpression(@"^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$", ErrorMessage = "Invalid ObjectionId format.")]
        public string ObjectionId { get; set; } = string.Empty;

        [RegularExpression(@"^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$", ErrorMessage = "Invalid UgtsReferenceNo format.")]
        public string? UgtsReferenceNo { get; set; }

        [Required(ErrorMessage = "TransactionReferenceNo is required.")]
        public string TransactionReferenceNo { get; set; } = string.Empty;

        [Required(ErrorMessage = "CustomerName is required.")]
        public string CustomerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "TelephoneNumber is required.")]
        public string? TelephoneNumber { get; set; }

        [Required(ErrorMessage = "Plate is required.")]
        [MaxLength(20, ErrorMessage = "Plate cannot exceed 20 characters.")]
        public string Plate { get; set; } = string.Empty;

        [Required(ErrorMessage = "ObjectionStatus is required.")]
        public string ObjectionStatus { get; set; } = string.Empty;

        [Required(ErrorMessage = "ObjectionDate is required.")]
        public string ObjectionDate { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; } = string.Empty;
    }
}