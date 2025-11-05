using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElectricityCuttingDownManagmentSystem.API.Models
{
    [Table("Cutting_Down_B")]
    public class Cutting_Down_B
    {
        [Key]
        [Column("Cutting_Down_B_Incident_ID")]
        public int CuttingDownBIncidentID { get; set; }

        [Required]
        [Column("Cable_Key")]
        public int CableKey { get; set; }

        [Required]
        [Column("Problem_Type_Key")]
        public int ProblemTypeKey { get; set; }

        [Required]
        [Column("CreateDate")]
        public DateTime CreateDate { get; set; }

        [Column("EndDate")]
        public DateTime? EndDate { get; set; }

        [Required]
        [Column("IsPlanned")]
        public bool IsPlanned { get; set; }

        [Required]
        [Column("IsGlobal")]
        public bool IsGlobal { get; set; }

        [Column("PlannedStartDTS")]
        public DateTime? PlannedStartDTS { get; set; }

        [Column("PlannedEndDTS")]
        public DateTime? PlannedEndDTS { get; set; }

        [Required]
        [Column("IsActive")]
        public bool IsActive { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("CreatedUser")]
        public string CreatedUser { get; set; } = "Source B user";

        [MaxLength(50)]
        [Column("UpdatedUser")]
        public string? UpdatedUser { get; set; }

        [Required]
        [Column("IsProcessed")]
        public bool IsProcessed { get; set; }
    }
}
