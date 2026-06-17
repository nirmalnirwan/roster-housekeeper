using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using roster_api_app.DTOs;
using roster_api_app.Services;

namespace roster_api_app.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FloorsController : ControllerBase
{
    private readonly IFloorService _service;

    public FloorsController(IFloorService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var floors = await _service.GetAllAsync();
        return Ok(floors);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var floor = await _service.GetByIdAsync(id);
        if (floor == null) return NotFound(new { error = "Floor not found" });
        return Ok(floor);
    }

    [HttpPost]
    public async Task<IActionResult> Create(FloorRequestDto dto)
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
    public async Task<IActionResult> Update(int id, FloorRequestDto dto)
    {
        try
        {
            await _service.UpdateAsync(id, dto);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { error = "Floor not found" });
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
