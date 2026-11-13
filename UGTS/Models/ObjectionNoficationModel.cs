using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace UGTS.Models
{
    public class ObjectionNoficationModel
    {
        [Required(ErrorMessage = "ObjectionId is required.")]
        [RegularExpression(@"^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$", ErrorMessage = "Invalid ObjectionId format.")]
        public string objection_id { get; set; } = string.Empty;

        [Required(ErrorMessage = "ugtsno is required.")]
        [RegularExpression(@"^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$", ErrorMessage = "Invalid UgtsReferenceNo format.")]
        public string? ugts_reference_no { get; set; }

        [Required(ErrorMessage = "TransactionReferenceNo is required.")]
        public string transaction_reference_no { get; set; } = string.Empty;

        [Required(ErrorMessage = "CustomerName is required.")]
        public string customer_name { get; set; } = string.Empty;

        //[Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [JsonPropertyName("email")]
        public string? email { get; set; }

        //[Required(ErrorMessage = "TelephoneNumber is required.")]
        public string? telephone_number { get; set; }

        [Required(ErrorMessage = "Plate is required.")]
        [MinLength(3, ErrorMessage = "Plate cannot be less than 3 characters.")]
        public string plate { get; set; } = string.Empty;

        [Required(ErrorMessage = "ObjectionStatus is required.")]
        public string objection_status { get; set; }

        [Required(ErrorMessage = "ObjectionDate is required.")]
        public string objection_date { get; set; } //tarihi uygun formatta kontrol et

        [Required(ErrorMessage = "Description is required.")]
        public string description { get; set; }
    }
}