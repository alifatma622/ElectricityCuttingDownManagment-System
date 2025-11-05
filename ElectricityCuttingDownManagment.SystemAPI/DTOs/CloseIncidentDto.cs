using System;
using System.ComponentModel.DataAnnotations;


namespace ElectricityCuttingDownManagmentSystem.API.DTOs
{
    public class CloseIncidentDto
    {
     
        [Required(ErrorMessage = "نوع المصدر مطلوب")]
        [RegularExpression("^(A|B)$", ErrorMessage = "نوع المصدر يجب أن يكون A أو B")]
        public string SourceType { get; set; } // "A" أو "B"

        [Required(ErrorMessage = "معرف الحادثة مطلوب")]
        [Range(1, int.MaxValue, ErrorMessage = "معرف الحادثة يجب أن يكون أكبر من 0")]
        public int IncidentID { get; set; }

        public DateTime? EndDate { get; set; }

        [Required(ErrorMessage = "اسم المستخدم مطلوب")]
        [MaxLength(50, ErrorMessage = "اسم المستخدم لا يمكن أن يتجاوز 50 حرف")]
        public string UpdatedUser { get; set; }

      
        public bool IsProcessed { get; set; } = true;
    }
}
