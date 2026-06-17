using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using roster_api_app.DTOs;
using roster_api_app.Services;

namespace roster_api_app.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LocationTypesController : ControllerBase
{
    private readonly ILocationTypeService _service;

    public LocationTypesController(ILocationTypeService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var locationTypes = await _service.GetAllAsync();
        return Ok(locationTypes);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var locationType = await _service.GetByIdAsync(id);
        if (locationType == null) return NotFound(new { error = "Location type not found" });
        return Ok(locationType);
    }

    [HttpPost]
    public async Task<IActionResult> Create(LocationTypeRequestDto dto)
    {
        try
        {
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, LocationTypeRequestDto dto)
    {
        try
        {
            await _service.UpdateAsync(id, dto);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { error = "Location type not found" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}
