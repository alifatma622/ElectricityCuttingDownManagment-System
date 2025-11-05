using System.ComponentModel.DataAnnotations;

namespace ElectricityCuttingDown.WebPortal.Models.ViewModels
{
    public class CreateIncidentViewModel
    {
        [Required(ErrorMessage = "Cabin Key أو Cable Key مطلوب")]
        [Range(1, int.MaxValue, ErrorMessage = "Key يجب أن يكون أكبر من 0")]
        public int ResourceKey { get; set; }  // Cabin_Key أو Cable_Key

        [Required(ErrorMessage = "نوع المشكلة مطلوب")]
        [Range(1, 13, ErrorMessage = "نوع المشكلة يجب أن يكون بين 1 و 13")]
        public int ProblemTypeKey { get; set; }

        [Required(ErrorMessage = "نوع المصدر مطلوب")]
        public string SourceType { get; set; }  // "A" أو "B"

        public DateTime CreateDate { get; set; } = DateTime.Now;
        public DateTime? EndDate { get; set; }

        public bool IsPlanned { get; set; } = false;
        public bool IsGlobal { get; set; } = false;

        public DateTime? PlannedStartDTS { get; set; }
        public DateTime? PlannedEndDTS { get; set; }

        public bool IsActive { get; set; } = true;

        [Required(ErrorMessage = "اسم المستخدم مطلوب")]
        [MaxLength(50, ErrorMessage = "اسم المستخدم لا يمكن أن يتجاوز 50 حرف")]
        public string CreatedUser { get; set; } = "Web User";

        public bool IsProcessed { get; set; } = false;
    }
}
