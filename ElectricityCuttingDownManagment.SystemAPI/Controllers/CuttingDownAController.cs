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
    public class CuttingDownAController : ControllerBase
    {
        private readonly IIncidentService _incidentService;
        private readonly ILogger<CuttingDownAController> _logger;

        public CuttingDownAController(
            IIncidentService incidentService,
            ILogger<CuttingDownAController> logger)
        {
            _incidentService = incidentService;
            _logger = logger;
        }


        ///create Incident
        ///post api/CuttingDownA/create
        [HttpPost("create")]
        [ProducesResponseType(typeof(IncidentResponseDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateIncident([FromBody] CreateIncidentDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _incidentService.CreateIncidentAsync(dto, "A");

                _logger.LogInformation("Cabin incident created: {IncidentID}", result.IncidentID);

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Argument error creating cabin incident");
                return BadRequest(new { error = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Resource not found creating cabin incident");
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating cabin incident");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }


        ///close Incident
        /// put api/CuttingDownA/close
        [HttpPut("close")]
        [ProducesResponseType(typeof(IncidentResponseDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]

        public async Task<IActionResult> CloseIncident([FromBody] CloseIncidentDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                dto.SourceType = "A";

                var result = await _incidentService.CloseIncidentAsync(dto);

                _logger.LogInformation("Cabin incident closed: {IncidentID}", result.IncidentID);

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Argument error closing cabin incident");
                return BadRequest(new { error = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Cabin incident not found: {IncidentID}", dto.IncidentID);
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error closing cabin incident");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }


        /// generate-test-data
        /// post api/CuttingDownA/generate-test-data?count=10
        [HttpPost("generate-test-data")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GenerateTestData([FromQuery] int count = 10)
        {
            try
            {
                if (count <= 0 || count > 1000)
                {
                    return BadRequest(new { error = "Count must be between 1 and 1000" });
                }

                var generated = await _incidentService.GenerateTestDataAsync(count, "A");

                _logger.LogInformation("Generated {Count} cabin test incidents", generated);

                return Ok(new
                {
                    message = "تم توليد بيانات الاختبار بنجاح",
                    count = generated,
                    sourceType = "Source A (Cabins)",
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating test data");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        ///health-check
        /// get api/CuttingDownA/health

        [HttpGet("health")]
        [ProducesResponseType(200)]
        public IActionResult HealthCheck()
        {
            return Ok(new
            {
                status = "Healthy",
                service = "Cutting Down A API (Cabins)",
                timestamp = DateTime.UtcNow
            });
        }
    }
}