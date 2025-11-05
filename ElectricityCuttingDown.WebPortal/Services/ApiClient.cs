using ElectricityCuttingDown.WebPortal.Models.ViewModels;
using System.Net.Http.Json;
namespace ElectricityCuttingDown.WebPortal.Services
{
    public interface IApiClient
    {
        Task<bool> CreateIncidentAsync(CreateIncidentViewModel model);
    }

    public class ApiClient : IApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ApiClient> _logger;
        private readonly string _apiUrl;

        public ApiClient(HttpClient httpClient, IConfiguration config, ILogger<ApiClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _apiUrl = config["ApiSettings:BaseUrl"] ?? "http://localhost:5051";
        }

        public async Task<bool> CreateIncidentAsync(CreateIncidentViewModel model)
        {
            try
            {
                _logger.LogInformation($"Creating incident via API - Source: {model.SourceType}, Resource: {model.ResourceKey}");

                // Map ViewModel to API DTO
                var payload = new
                {
                    resourceKey = model.ResourceKey,
                    problemTypeKey = model.ProblemTypeKey,
                    sourceType = model.SourceType,  // "A" أو "B"
                    createDate = model.CreateDate,
                    endDate = model.EndDate,
                    isPlanned = model.IsPlanned,
                    isGlobal = model.IsGlobal,
                    plannedStartDTS = model.PlannedStartDTS,
                    plannedEndDTS = model.PlannedEndDTS,
                    isActive = model.IsActive,
                    createdUser = model.CreatedUser,
                    isProcessed = model.IsProcessed
                };

                // اختار الـ endpoint حسب الـ source
                string endpoint = model.SourceType == "A"
                    ? $"{_apiUrl}/api/CuttingDownA/create"
                    : $"{_apiUrl}/api/CuttingDownB/create";

                _logger.LogInformation($"Sending POST to: {endpoint}");

                var response = await _httpClient.PostAsJsonAsync(endpoint, payload);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation($"✓ Incident created successfully: {result}");
                    return true;
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"✗ API Error: {response.StatusCode} - {error}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"✗ Exception creating incident: {ex.Message}");
                return false;
            }
        }
    }
}
