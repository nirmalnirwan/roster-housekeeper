using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using roster_api_app.DTOs;
using roster_api_app.Services;

namespace roster_api_app.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ApartmentsController : ControllerBase
{
    private readonly IApartmentService _service;

    public ApartmentsController(IApartmentService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var apartments = await _service.GetAllAsync();
        return Ok(apartments);
    }

    [HttpGet("by-floor/{floorId}")]
    public async Task<IActionResult> GetByFloor(int floorId)
    {
        var apartments = await _service.GetByFloorAsync(floorId);
        return Ok(apartments);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var apartment = await _service.GetByIdAsync(id);
        if (apartment == null) return NotFound(new { error = "Apartment not found" });
        return Ok(apartment);
    }

    [HttpPost]
    public async Task<IActionResult> Create(ApartmentRequestDto dto)
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
    public async Task<IActionResult> Update(int id, ApartmentRequestDto dto)
    {
        try
        {
            await _service.UpdateAsync(id, dto);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { error = "Apartment not found" });
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
