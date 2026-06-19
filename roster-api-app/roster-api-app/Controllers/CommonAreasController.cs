using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using roster_api_app.DTOs;
using roster_api_app.Services;

namespace roster_api_app.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CommonAreasController : ControllerBase
{
    private readonly ICommonAreaService _service;

    public CommonAreasController(ICommonAreaService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var areas = await _service.GetAllAsync();
        return Ok(areas);
    }

    [HttpGet("by-floor/{floorId}")]
    public async Task<IActionResult> GetByFloor(int floorId)
    {
        var areas = await _service.GetByFloorAsync(floorId);
        return Ok(areas);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var area = await _service.GetByIdAsync(id);
        if (area == null) return NotFound(new { error = "Common area not found" });
        return Ok(area);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CommonAreaRequestDto dto)
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
    public async Task<IActionResult> Update(int id, CommonAreaRequestDto dto)
    {
        try
        {
            await _service.UpdateAsync(id, dto);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { error = "Common area not found" });
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
