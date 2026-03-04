using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkOrders.Api.Data;

namespace WorkOrders.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WorkOrdersController : ControllerBase
{
    private readonly AppDbContext _db;

    public WorkOrdersController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> List()
    {
        var items = await _db.WorkOrders
            .OrderByDescending(w => w.UpdatedAt)
            .ToListAsync();

        return Ok(items);
    }
}