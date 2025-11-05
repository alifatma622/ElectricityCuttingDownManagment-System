using ElectricityCuttingDown.WebPortal.Data;
using ElectricityCuttingDown.WebPortal.Models.Entities;
using ElectricityCuttingDown.WebPortal.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ElectricityCuttingDown.WebPortal.Services
{
    public class IncidentService : IIncidentService
    {
        private readonly Electricity_FTAContext _ftaContext;
        private readonly IApiClient _apiClient;
        private readonly ILogger<IncidentService> _logger;

        public IncidentService(
            Electricity_FTAContext ftaContext,
            IApiClient apiClient,
            ILogger<IncidentService> logger)
        {
            _ftaContext = ftaContext;
            _apiClient = apiClient;
            _logger = logger;
        }

        public async Task<IncidentSearchViewModel> SearchIncidentsAsync(
            int? source, int? problemType, string status,
            DateTime? startDate, DateTime? endDate,
            int pageNumber, int pageSize)
        {
            var query = _ftaContext.Cutting_Down_Header.AsQueryable();

            if (source.HasValue)
                query = query.Where(x => x.Channel_Key == source.Value);

            if (problemType.HasValue)
                query = query.Where(x => x.Cutting_Down_Problem_Type_Key == problemType.Value);

            if (!string.IsNullOrEmpty(status))
            {
                if (status == "Open")
                    query = query.Where(x => x.ActualEndDate == null);
                else if (status == "Closed")
                    query = query.Where(x => x.ActualEndDate != null);
            }

            if (startDate.HasValue)
                query = query.Where(x => x.ActualCreateDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(x => x.ActualCreateDate <= endDate.Value);

            var totalCount = await query.CountAsync();

            var incidents = await query
                .OrderByDescending(x => x.ActualCreateDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new IncidentDto
                {
                    Cutting_Down_Key = x.Cutting_Down_Key,
                    Cutting_Down_Incident_ID = x.Cutting_Down_Incident_ID,
                    Channel = x.Channel_Key == 1 ? "Source A" : "Source B",
                    ProblemType = "---",
                    ActualCreateDate = x.ActualCreateDate,
                    ActualEndDate = x.ActualEndDate,
                    ImpactedCustomers = 0
                })
                .ToListAsync();

            return new IncidentSearchViewModel
            {
                Results = incidents,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<DashboardViewModel> GetDashboardDataAsync()
        {
            var total = await _ftaContext.Cutting_Down_Header.CountAsync();
            var open = await _ftaContext.Cutting_Down_Header
                .Where(x => x.ActualEndDate == null)
                .CountAsync();
            var closed = total - open;

            var recent = await _ftaContext.Cutting_Down_Header
                .OrderByDescending(x => x.ActualCreateDate)
                .Take(5)
                .Select(x => new IncidentDto
                {
                    Cutting_Down_Key = x.Cutting_Down_Key,
                    Cutting_Down_Incident_ID = x.Cutting_Down_Incident_ID,
                    Channel = x.Channel_Key == 1 ? "Source A" : "Source B",
                    ActualCreateDate = x.ActualCreateDate,
                    ActualEndDate = x.ActualEndDate
                })
                .ToListAsync();

            return new DashboardViewModel
            {
                TotalIncidents = total,
                OpenIncidents = open,
                ClosedIncidents = closed,
                IgnoredIncidents = 0,
                RecentIncidents = recent
            };
        }

        // ====== Create Incident (تستدعي الـ API) ======
        public async Task<bool> CreateIncidentAsync(CreateIncidentViewModel model)
        {
            try
            {
                _logger.LogInformation($"Creating incident - Source: {model.SourceType}");

                // استدعاء الـ API
                var success = await _apiClient.CreateIncidentAsync(model);

                if (success)
                {
                    _logger.LogInformation("✓ Incident created successfully via API");
                }
                else
                {
                    _logger.LogError("✗ Failed to create incident via API");
                }

                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError($"✗ Error creating incident: {ex.Message}");
                return false;
            }
        }
    }

}

