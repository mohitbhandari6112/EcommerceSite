using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Ecommerce_Razoer.Model
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [DisplayName("Display Name")]
        [MaxLength(20)]
        public string Name { get; set; }
        [DisplayName("Display Order")]
        [Range(1, 100)]
        public int DisplayOrder { get; set; }
    }
}
