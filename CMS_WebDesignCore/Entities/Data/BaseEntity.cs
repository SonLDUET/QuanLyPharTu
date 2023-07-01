using System.ComponentModel.DataAnnotations;

namespace CMS_WebDesignCore.Entities
{
    public class BaseEntity
    {
        [Key]
        public int Id { get; set; } 
    }
}