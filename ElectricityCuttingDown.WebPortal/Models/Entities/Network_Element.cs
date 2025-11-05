namespace ElectricityCuttingDown.WebPortal.Models.Entities
{
    public class Network_Element
    {
        public int Network_Element_Key { get; set; }
        public string Network_Element_Name { get; set; }
        public int Network_Element_Type_Key { get; set; }
        public int? Parent_Network_Element_Key { get; set; }
        public int Network_Element_Hierarchy_Path_Key { get; set; }
    }
}
