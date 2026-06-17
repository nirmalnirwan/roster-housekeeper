using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using roster_api_app.DTOs;
using roster_api_app.Services;

namespace roster_api_app.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BuildingBlocksController : ControllerBase
{
    private readonly IBuildingBlockService _service;

    public BuildingBlocksController(IBuildingBlockService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var buildingBlocks = await _service.GetAllAsync();
        return Ok(buildingBlocks);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var buildingBlock = await _service.GetByIdAsync(id);
        if (buildingBlock == null) return NotFound(new { error = "Building block not found" });
        return Ok(buildingBlock);
    }

    [HttpPost]
    public async Task<IActionResult> Create(BuildingBlockRequestDto dto)
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
    public async Task<IActionResult> Update(int id, BuildingBlockRequestDto dto)
    {
        try
        {
            await _service.UpdateAsync(id, dto);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { error = "Building block not found" });
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
