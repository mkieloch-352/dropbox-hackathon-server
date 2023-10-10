using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace webapi.Models
{
    [Table("Contract")]
    public class Contract : BaseEntity, IBaseEntity
    {
        public string EditUrl { get; set; }
        public string? RequestorName { get; set; }
        public string? RequestorEmail { get; set; }
        public string? SignerName { get; set; }
        public string? SignerEmail { get; set; }
        public string? SignerPhoneNumber { get; set; }
        public string? SignatureUrl { get; set; }
        public string Content { get; set; }
        public string TemplateId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }

        public Contract()
        {
            CreatedDate = DateTime.UtcNow;
            ModifiedDate = CreatedDate;
        }
    }
}
