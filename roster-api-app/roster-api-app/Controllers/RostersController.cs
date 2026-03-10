using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using roster_api_app.DTOs;
using roster_api_app.Services;

namespace roster_api_app.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RostersController : ControllerBase
{
    private readonly IRosterService _service;

    public RostersController(IRosterService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var dtos = await _service.GetAllAsync();
        return Ok(dtos);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var dto = await _service.GetByIdAsync(id);
        if (dto == null) return NotFound();
        return Ok(dto);
    }

    [HttpPost]
    public async Task<IActionResult> Create(RosterDto dto)
    {
        var created = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, RosterDto dto)
    {
        await _service.UpdateAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }

    [HttpGet("{id}/export/pdf")]
    public async Task<IActionResult> ExportPdf(int id)
    {
        var bytes = await _service.ExportPdfAsync(id);
        return File(bytes, "application/pdf", $"roster_{id}.pdf");
    }

    [HttpGet("{id}/export/excel")]
    public async Task<IActionResult> ExportExcel(int id)
    {
        var bytes = await _service.ExportExcelAsync(id);
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"roster_{id}.xlsx");
    }
}