using ElectricityCuttingDown.WebPortal.Models;
using Microsoft.AspNetCore.Mvc;
using ElectricityCuttingDown.WebPortal.Services;
using System.Diagnostics;

namespace ElectricityCuttingDown.WebPortal.Controllers
{
    public class HomeController : Controller
    {
        private readonly IIncidentService _incidentService;

        public HomeController(IIncidentService incidentService)
        {
            _incidentService = incidentService;
        }

        public async Task<IActionResult> Index()
        {
            var dashboard = await _incidentService.GetDashboardDataAsync();
            return View(dashboard);
        }
    }
}
