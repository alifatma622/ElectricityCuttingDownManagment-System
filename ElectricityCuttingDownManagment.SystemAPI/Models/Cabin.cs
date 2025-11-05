using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElectricityCuttingDownManagmentSystem.API.Models
{
    [Table("Cabin")]
    public class Cabin
    {

        [Key]
        public int Cabin_Key { get; set; }

        [Required]
        public int Tower_Key { get; set; }

        [Required]
        [MaxLength(100)]
        public string Cabin_Name { get; set; } = string.Empty;
    }
}
