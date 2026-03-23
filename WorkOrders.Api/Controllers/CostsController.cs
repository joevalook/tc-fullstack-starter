using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkOrders.Api.Dtos;
using WorkOrders.Api.Services.Interfaces;

namespace WorkOrders.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CostsController : ControllerBase
{
    private readonly ICostService _costService;

    public CostsController(ICostService costService)
    {
        _costService = costService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CostResponse>>> List()
    {
        var items = await _costService.ListAsync();

        return Ok(items);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CostResponse>> GetById(int id)
    {
        var item = await _costService.GetByIdAsync(id);

        if (item == null)
        {
            return NotFound();
        }

        return Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<CostResponse>> Post(CreateCostRequest request)
    {
        var result = await _costService.CreateAsync(request);

        if (result.InvalidWorkOrderId)
        {
            return BadRequest(new { message = "WorkOrder ID is invalid" });
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Cost!.Id }, result.Cost);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<CostResponse>> Put(int id, UpdateCostRequest request)
    {
        var result = await _costService.UpdateAsync(id, request);

        if (result.NotFound)
        {
            return NotFound();
        }

        if (result.InvalidWorkOrderId)
        {
            return BadRequest(new { message = "WorkOrder ID is invalid" });
        }

        return Ok(result.Cost);
    }

    [HttpPatch("{id:int}")]
    public async Task<ActionResult<CostResponse>> Patch(int id, PatchCostRequest request)
    {
        var result = await _costService.PatchAsync(id, request);

        if (result.NotFound)
        {
            return NotFound();
        }

        if (result.InvalidWorkOrderId)
        {
            return BadRequest(new { message = "WorkOrder ID is invalid" });
        }

        return Ok(result.Cost);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _costService.DeleteAsync(id);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}