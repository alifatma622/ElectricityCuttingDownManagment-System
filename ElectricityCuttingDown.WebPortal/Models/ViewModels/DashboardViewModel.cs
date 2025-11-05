namespace ElectricityCuttingDown.WebPortal.Models.ViewModels
{
    public class DashboardViewModel
    {

        public int TotalIncidents { get; set; }
        public int OpenIncidents { get; set; }
        public int ClosedIncidents { get; set; }
        public int IgnoredIncidents { get; set; }
        public List<IncidentDto> RecentIncidents { get; set; } = new();
    }
}
