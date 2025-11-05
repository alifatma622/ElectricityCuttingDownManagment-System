namespace ElectricityCuttingDown.WebPortal.Models.ViewModels
{
    public class IgnoredOutagesViewModel
    {
        public List<IgnoredOutageDto> IgnoredOutages { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    public class IgnoredOutageDto
    {
        public string Cutting_Down_Incident_ID { get; set; }
        public DateTime ActualCreateDate { get; set; }
        public DateTime SynchCreateDate { get; set; }
        public string Cable_Name { get; set; }
        public string Cabin_Name { get; set; }
        public string CreatedUser { get; set; }
    }
}
