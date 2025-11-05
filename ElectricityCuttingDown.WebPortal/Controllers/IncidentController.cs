
using ElectricityCuttingDown.WebPortal.Data;
using ElectricityCuttingDown.WebPortal.Models.ViewModels;
using ElectricityCuttingDown.WebPortal.Services;
using ElectricityCuttingDown.WebPortal.Data;
using ElectricityCuttingDown.WebPortal.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ElectricityCuttingDown.Web.Controllers
{
    public class IncidentController : Controller
    {
        private readonly IIncidentService _incidentService;
        private readonly Electricity_FTAContext _context;
        private readonly ILogger<IncidentController> _logger;

        public IncidentController(
            IIncidentService incidentService,
            Electricity_FTAContext context,
            ILogger<IncidentController> logger)
        {
            _incidentService = incidentService;
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Search(
            int? source, int? problemType, string status,
            DateTime? startDate, DateTime? endDate, int pageNumber = 1)
        {
            var result = await _incidentService.SearchIncidentsAsync(
                source, problemType, status, startDate, endDate, pageNumber, 20);

            return View(result);
        }

        public IActionResult Add()
        {
            return View(new CreateIncidentViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(CreateIncidentViewModel model)
        {
            try
            {
                // Validation
                if (string.IsNullOrEmpty(model.SourceType))
                {
                    ModelState.AddModelError("SourceType", "Please select a source");
                }

                if (model.ProblemTypeKey <= 0)
                {
                    ModelState.AddModelError("ProblemTypeKey", "Please select a problem type");
                }

                if (model.ResourceKey <= 0)
                {
                    ModelState.AddModelError("ResourceKey", "Please enter resource key (Cabin or Cable)");
                }

                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                // Call API to create incident
                var success = await _incidentService.CreateIncidentAsync(model);

                if (success)
                {
                    TempData["Success"] = $"✓ Incident created successfully! (Source: {model.SourceType})";
                    _logger.LogInformation("Incident created by user via API");
                    return View(new CreateIncidentViewModel());
                }
                else
                {
                    ModelState.AddModelError("", "Error creating incident via API. Please check the API is running.");
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in Add action: {ex.Message}");
                ModelState.AddModelError("", $"Error: {ex.Message}");
                return View(model);
            }
        }
        public async Task<IActionResult> IgnoredOutages(int pageNumber = 1)
        {
            try
            {
                var pageSize = 20;
                var ignored = await _context.Cutting_Down_Ignored
                    .OrderByDescending(x => x.ActualCreateDate)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(x => new IgnoredOutageDto
                    {
                        Cutting_Down_Incident_ID = x.Cutting_Down_Incident_ID,
                        ActualCreateDate = x.ActualCreateDate,
                        SynchCreateDate = x.SynchCreateDate,
                        Cable_Name = x.Cable_Name,
                        Cabin_Name = x.Cabin_Name,
                        CreatedUser = x.CreatedUser
                    })
                    .ToListAsync();

                var total = await _context.Cutting_Down_Ignored.CountAsync();

                var model = new IgnoredOutagesViewModel
                {
                    IgnoredOutages = ignored,
                    TotalCount = total,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading ignored outages: {ex.Message}");
                return View(new IgnoredOutagesViewModel());
            }
        }
    }
}
