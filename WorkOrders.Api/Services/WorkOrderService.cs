using Microsoft.EntityFrameworkCore;
using WorkOrders.Api.Data;
using WorkOrders.Api.Dtos;
using WorkOrders.Api.Models;
using WorkOrders.Api.Services.Interfaces;

namespace WorkOrders.Api.Services;

public class WorkOrderService : IWorkOrderService
{
    private readonly AppDbContext _db;

    public WorkOrderService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<WorkOrderListItemResponse>> ListAsync()
    {
        var items = await _db.WorkOrders
            .AsNoTracking()
            .OrderByDescending(w => w.UpdatedAt)
            .ToListAsync();

        return items.Select(w => new WorkOrderListItemResponse
        {
            Id = w.Id,
            Title = w.Title,
            Description = w.Description,
            Status = w.Status,
            CreatedAt = w.CreatedAt,
            UpdatedAt = w.UpdatedAt
        });
    }

    public async Task<WorkOrderResponse?> GetByIdAsync(int id)
    {
        var item = await _db.WorkOrders
            .AsNoTracking()
            .Select(w => new WorkOrderResponse
            {
                Id = w.Id,
                Title = w.Title,
                Description = w.Description,
                Status = w.Status,
                CreatedAt = w.CreatedAt,
                UpdatedAt = w.UpdatedAt,
                Costs = w.Costs.Select(c => new CostResponse
                {
                    Id = c.Id,
                    Name = c.Name,
                    Amount = c.Amount,
                    CreatedAt = c.CreatedAt
                }).ToList()
            })
            .FirstOrDefaultAsync(w => w.Id == id);

        return item;
    }

    public async Task<WorkOrderResponse> CreateAsync(CreateWorkOrderRequest request)
    {
        var workOrder = new WorkOrder
        {
            Title = request.Title,
            Description = request.Description ?? "",
            Status = request.Status,
            UpdatedAt = DateTime.UtcNow
        };

        _db.WorkOrders.Add(workOrder);
        await _db.SaveChangesAsync();

        await _db.Entry(workOrder)
            .Collection(w => w.Costs)
            .LoadAsync();

        return new WorkOrderResponse
        {
            Id = workOrder.Id,
            Title = workOrder.Title,
            Description = workOrder.Description,
            Status = workOrder.Status,
            CreatedAt = workOrder.CreatedAt,
            UpdatedAt = workOrder.UpdatedAt,
            Costs = workOrder.Costs.Select(c => new CostResponse
            {
                Id = c.Id,
                Name = c.Name,
                Amount = c.Amount,
                CreatedAt = c.CreatedAt
            }).ToList()
        };
    }

    public async Task<WorkOrderResponse?> UpdateAsync(int id, UpdateWorkOrderRequest request)
    {
        var item = await _db.WorkOrders
            .Include(w => w.Costs)
            .FirstOrDefaultAsync(w => w.Id == id);

        if (item == null)
            return null;

        item.Title = request.Title;
        item.Description = request.Description ?? "";
        item.Status = request.Status;
        item.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return new WorkOrderResponse
        {
            Id = item.Id,
            Title = item.Title,
            Description = item.Description,
            Status = item.Status,
            CreatedAt = item.CreatedAt,
            UpdatedAt = item.UpdatedAt,
            Costs = item.Costs.Select(c => new CostResponse
            {
                Id = c.Id,
                Name = c.Name,
                Amount = c.Amount,
                CreatedAt = c.CreatedAt
            }).ToList()
        };
    }

    public async Task<WorkOrderResponse?> PatchAsync(int id, PatchWorkOrderRequest request)
    {
        var item = await _db.WorkOrders
            .Include(w => w.Costs)
            .FirstOrDefaultAsync(w => w.Id == id);

        if (item == null)
            return null;

        if (request.Title != null)
            item.Title = request.Title;

        if (request.Description != null)
            item.Description = request.Description;

        if (request.Status != null)
            item.Status = request.Status;

        item.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return new WorkOrderResponse
        {
            Id = item.Id,
            Title = item.Title,
            Description = item.Description,
            Status = item.Status,
            CreatedAt = item.CreatedAt,
            UpdatedAt = item.UpdatedAt,
            Costs = item.Costs.Select(c => new CostResponse
            {
                Id = c.Id,
                Name = c.Name,
                Amount = c.Amount,
                CreatedAt = c.CreatedAt
            }).ToList()
        };
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var item = await _db.WorkOrders.FindAsync(id);

        if (item == null)
            return false;

        _db.WorkOrders.Remove(item);
        await _db.SaveChangesAsync();

        return true;
    }
}