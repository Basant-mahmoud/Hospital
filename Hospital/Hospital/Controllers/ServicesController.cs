using AutoMapper;
using Hospital.Application.DTO.ServiceDTOS;
using Hospital.Application.Interfaces.Repos;
using Hospital.Application.Interfaces.Services;
using Hospital.Domain.Models;
using Hospital.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;

namespace Hospital.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ServicesController : ControllerBase
    {
        private readonly IServiceService _serviceService;
        private readonly ILogger<ServicesController> _logger;

        public ServicesController(IServiceService serviceService, ILogger<ServicesController> logger)
        {
            _serviceService = serviceService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("get all services called at {time}", DateTime.Now);

            var services = await _serviceService.GetAllAsync();
            return Ok(services);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            _logger.LogInformation("get service by id  called at {time}", DateTime.Now);

            var service = await _serviceService.GetByIdAsync(id);
            if (service == null)
                return NotFound();

            return Ok(service);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Create([FromBody] CreateServiceDto dto)
        {
            _logger.LogInformation("create services called at {time}", DateTime.Now);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _serviceService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.ServiceId }, created);
        }

        [HttpPost("add-services-from-excel")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> AddServicesFromExcel(IFormFile file)
        {
            _logger.LogInformation("all services by excel sheet services called at {time}", DateTime.Now);

            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var services = new List<CreateServiceDto>();

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                stream.Position = 0;

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                using (var package = new ExcelPackage(stream))
                {
                    var sheet = package.Workbook.Worksheets.FirstOrDefault();
                    if (sheet == null)
                        return BadRequest("Excel file has no sheets.");

                    for (int row = 2; row <= sheet.Dimension.End.Row; row++)
                    {
                        var dto = new CreateServiceDto
                        {
                            Name = sheet.Cells[row, 1].Text,
                            Description = sheet.Cells[row, 2].Text,
                            ImageURL = sheet.Cells[row, 3].Text,
                        };

                        var branches = sheet.Cells[row, 4].Text;
                        if (!string.IsNullOrEmpty(branches))
                        {
                            dto.BranchesID = branches
                                .Split(",", StringSplitOptions.RemoveEmptyEntries)
                                .Select(id => new BranchIdDTO { BranchId = int.Parse(id.Trim()) })
                                .ToList();
                        }

                        services.Add(dto);
                    }
                }
            }

            var results = new List<object>();
            foreach (var service in services)
            {
                try
                {
                    var created = await _serviceService.CreateAsync(service);

                    results.Add(new
                    {
                        service.Name,
                        Status = "Success",
                        ServiceId = created.ServiceId
                    });
                }
                catch (Exception ex)
                {
                    results.Add(new
                    {
                        service.Name,
                        Status = "Failed",
                        Error = ex.Message
                    });
                }
            }

            return Ok(results);
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Update([FromBody] UpdateServiceDto dto)
        {
            _logger.LogInformation("update services called at {time}", DateTime.Now);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _serviceService.UpdateAsync(dto);
            return Ok("updated Successfully");
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("delete services called at {time}", DateTime.Now);

            var deleted = await _serviceService.DeleteAsync(id);
            if (!deleted) return NotFound();
            return Ok("Deleted Successfully");
        }
    }
}