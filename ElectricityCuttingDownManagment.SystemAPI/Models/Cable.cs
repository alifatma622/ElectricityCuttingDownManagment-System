using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElectricityCuttingDownManagmentSystem.API.Models
{
    [Table("Cable")]
    public class Cable
    {

        [Key]
        public int Cable_Key { get; set; }

        [Required]
        public int Cabin_Key { get; set; }

        [Required]
        [MaxLength(100)]
        public string Cable_Name { get; set; } = string.Empty;
    }
}
