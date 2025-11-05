namespace ElectricityCuttingDown.WebPortal.Models.Entities
{
    public class Cutting_Down_Detail
    {
        public int Cutting_Down_Detail_Key { get; set; }
        public int Cutting_Down_Key { get; set; }
        public int Network_Element_Key { get; set; }
        public DateTime ActualCreateDate { get; set; }
        public DateTime? ActualEndDate { get; set; }
        public int ImpactedCustomers { get; set; }
    }
}
