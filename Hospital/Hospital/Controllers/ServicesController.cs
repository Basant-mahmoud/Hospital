using AutoMapper;
using Hospital.Application.DTO.ServiceDTOS;
using Hospital.Application.Interfaces.Repos;
using Hospital.Application.Interfaces.Services;
using Hospital.Domain.Models;
using Hospital.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;

namespace Hospital.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServicesController : ControllerBase
    {
        private readonly IServiceService _serviceService;

        public ServicesController(IServiceService serviceService)
        {
            _serviceService = serviceService;
        }

        // GET: api/services
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var services = await _serviceService.GetAllAsync();
            return Ok(services);
        }

        // GET: api/services/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var service = await _serviceService.GetByIdAsync(id);
            if (service == null)
                return NotFound();

            return Ok(service);
        }

        // POST: api/services
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateServiceDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _serviceService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.ServiceId }, created);
        }

        [HttpPost("add-services-from-excel")]
        public async Task<IActionResult> AddServicesFromExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var services = new List<CreateServiceDto>();

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                stream.Position = 0;

                // EPPlus النسخة القديمة ≤ 5.8
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

                        // Branch IDs → comma separated (مثال: "1,2,3")
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

        // PUT: api/services
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateServiceDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _serviceService.UpdateAsync(dto);
            return Ok("updated Successfully");
        }

        // DELETE: api/services/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _serviceService.DeleteAsync(id);
            if (!deleted) return NotFound();
            return Ok("Deleted Successfully");
        }
    }
}