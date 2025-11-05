namespace ElectricityCuttingDown.WebPortal.Models.ViewModels
{
    public class IncidentSearchViewModel
    {
        public int? Source { get; set; }
        public int? ProblemType { get; set; }
        public string Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public List<IncidentDto> Results { get; set; } = new();
        public int TotalCount { get; set; }
    }

    public class IncidentDto
    {
        public int Cutting_Down_Key { get; set; }
        public string Cutting_Down_Incident_ID { get; set; }
        public string Channel { get; set; }
        public string ProblemType { get; set; }
        public DateTime ActualCreateDate { get; set; }
        public DateTime? ActualEndDate { get; set; }
        public string Status => ActualEndDate == null ? "Open" : "Closed";
        public int ImpactedCustomers { get; set; }
    }
}
