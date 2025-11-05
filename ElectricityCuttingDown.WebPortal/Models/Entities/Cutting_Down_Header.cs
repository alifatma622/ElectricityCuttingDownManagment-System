namespace ElectricityCuttingDown.WebPortal.Models.Entities
{
    public class Cutting_Down_Header
    {

        public int Cutting_Down_Key { get; set; }
        public string Cutting_Down_Incident_ID { get; set; }
        public int Channel_Key { get; set; }
        public int Cutting_Down_Problem_Type_Key { get; set; }
        public DateTime ActualCreateDate { get; set; }
        public DateTime? ActualEndDate { get; set; }
        public DateTime SynchCreateDate { get; set; }
        public DateTime? SynchUpdateDate { get; set; }
        public bool IsPlanned { get; set; }
        public bool IsGlobal { get; set; }
        public bool IsActive { get; set; }
        public DateTime? PlannedStartDTS { get; set; }
        public DateTime? PlannedEndDTS { get; set; }
        public string CreateSystemUserID { get; set; }
        public string UpdateSystemUserID { get; set; }
    }
}
