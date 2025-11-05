using ElectricityCuttingDownManagmentSystem.API;
using ElectricityCuttingDownManagmentSystem.API.DTOs;
using ElectricityCuttingDownManagmentSystem.API.Interfaces;
using ElectricityCuttingDownManagmentSystem.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;


namespace ElectricityCuttingDownManagmentSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class CuttingDownBController : ControllerBase
    {
        private readonly IIncidentService _incidentService;
        private readonly ILogger<CuttingDownBController> _logger;

        public CuttingDownBController(
            IIncidentService incidentService,
            ILogger<CuttingDownBController> logger)
        {
            _incidentService = incidentService;
            _logger = logger;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateIncident([FromBody] CreateIncidentDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _incidentService.CreateIncidentAsync(dto, "B");
                _logger.LogInformation("Cable incident created: {IncidentID}", result.IncidentID);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating cable incident");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        [HttpPut("close")]
        public async Task<IActionResult> CloseIncident([FromBody] CloseIncidentDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                dto.SourceType = "B";
                var result = await _incidentService.CloseIncidentAsync(dto);
                _logger.LogInformation("Cable incident closed: {IncidentID}", result.IncidentID);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error closing cable incident");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        [HttpPost("generate-test-data")]
        public async Task<IActionResult> GenerateTestData([FromQuery] int count = 10)
        {
            try
            {
                if (count <= 0 || count > 1000)
                    return BadRequest(new { error = "Count must be between 1 and 1000" });

                var generated = await _incidentService.GenerateTestDataAsync(count, "B");
                _logger.LogInformation("Generated {Count} cable test incidents", generated);

                return Ok(new
                {
                    message = "تم توليد بيانات الاختبار بنجاح",
                    count = generated,
                    sourceType = "Source B (Cables)",
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating test data");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        [HttpGet("health")]
        public IActionResult HealthCheck()
        {
            return Ok(new
            {
                status = "Healthy",
                service = "Cutting Down B API (Cables)",
                timestamp = DateTime.UtcNow
            });
        }
    }
}
