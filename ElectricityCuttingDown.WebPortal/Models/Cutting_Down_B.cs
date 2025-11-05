namespace ElectricityCuttingDown.WebPortal.Models
{
    public class Cutting_Down_B
    {
        public int Cutting_Down_B_Incident_ID { get; set; }
        public int Cable_Key { get; set; }
        public int Problem_Type_Key { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsProcessed { get; set; }
        public bool IsPlanned { get; set; }
        public bool IsGlobal { get; set; }
        public DateTime? PlannedStartDTS { get; set; }
        public DateTime? PlannedEndDTS { get; set; }
    }
}
