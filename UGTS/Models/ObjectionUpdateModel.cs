namespace UGTS.Models
{
    public class ObjectionUpdateModel
    {
        public string ObjectionId { get; set; } = string.Empty;
        public string ObjectionStatus { get; set; } = string.Empty; 
        public string Answer { get; set; } = string.Empty;
        public string Responder { get; set; } = string.Empty;
    }

    public class ObjectionUpdateFormModel
    {
        public string Request { get; set; } = string.Empty;
        public IFormFile? Attachment { get; set; }
    }
}
