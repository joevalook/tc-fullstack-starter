using Microsoft.AspNetCore.Mvc;
using WorkOrders.Api.Dtos;
using WorkOrders.Api.Services.Interfaces;

namespace WorkOrders.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WorkOrdersController : ControllerBase
{
    private readonly IWorkOrderService _workOrderService;

    public WorkOrdersController(IWorkOrderService workOrderService)
    {
        _workOrderService = workOrderService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<WorkOrderListItemResponse>>> List()
    {
        var items = await _workOrderService.ListAsync();

        return Ok(items);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<WorkOrderResponse>> GetById(int id)
    {
        var item = await _workOrderService.GetByIdAsync(id);

        if (item == null)
            return NotFound();

        return Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<WorkOrderResponse>> Post(CreateWorkOrderRequest request)
    {
        var item = await _workOrderService.CreateAsync(request);

        return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<WorkOrderResponse>> Put(int id, UpdateWorkOrderRequest request)
    {
        var item = await _workOrderService.UpdateAsync(id, request);

        if (item == null)
            return NotFound();

        return Ok(item);
    }

    [HttpPatch("{id:int}")]
    public async Task<ActionResult<WorkOrderResponse>> Patch(int id, PatchWorkOrderRequest request)
    {
        var item = await _workOrderService.PatchAsync(id, request);

        if (item == null)
            return NotFound();

        return Ok(item);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _workOrderService.DeleteAsync(id);

        if (!deleted)
            return NotFound();

        return NoContent();
    }
}