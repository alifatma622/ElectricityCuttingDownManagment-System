using ElectricityCuttingDown.WebPortal.Models.ViewModels;

namespace ElectricityCuttingDown.WebPortal.Services
{
    public interface IIncidentService
    {


        Task<IncidentSearchViewModel> SearchIncidentsAsync(
            int? source, int? problemType, string status,
            DateTime? startDate, DateTime? endDate,
            int pageNumber, int pageSize);

        Task<DashboardViewModel> GetDashboardDataAsync();

        Task<bool> CreateIncidentAsync(CreateIncidentViewModel model);
    }
}
