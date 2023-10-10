using System.ComponentModel.DataAnnotations;

namespace webapi.Models
{
    public class BaseEntity : IBaseEntity
    {
        [Key]
        public Guid Id { get; set; }
    }
}
