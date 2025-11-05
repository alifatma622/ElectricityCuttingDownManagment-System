using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElectricityCuttingDownManagmentSystem.API.Models
{
    [Table("Cutting_Down_A")]
    public class Cutting_Down_A
    {
        [Key]
        [Column("Cutting_Down_A_Incident_ID")]
        public int CuttingDownAIncidentID { get; set; }

        [Required]
        [Column("Cabin_Key")]
        public int CabinKey { get; set; }

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
        public string CreatedUser { get; set; } = "Source A user";

        [MaxLength(50)]
        [Column("UpdatedUser")]
        public string? UpdatedUser { get; set; }

        [Required]
        [Column("IsProcessed")]
        public bool IsProcessed { get; set; }
    }
}
