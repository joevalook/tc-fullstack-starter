using Microsoft.EntityFrameworkCore;
using WorkOrders.Api.Data;
using WorkOrders.Api.Dtos;
using WorkOrders.Api.Models;
using WorkOrders.Api.Services.Interfaces;

namespace WorkOrders.Api.Services;

public class CostService : ICostService
{
    private readonly AppDbContext _db;

    public CostService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<CostResponse>> ListAsync()
    {
        return await _db.Costs
            .AsNoTracking()
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => new CostResponse
            {
                Id = c.Id,
                Name = c.Name,
                Amount = c.Amount,
                WorkOrderId = c.WorkOrderId,
                CreatedAt = c.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<CostResponse?> GetByIdAsync(int id)
    {
        return await _db.Costs
            .AsNoTracking()
            .Select(c => new CostResponse
            {
                Id = c.Id,
                Name = c.Name,
                Amount = c.Amount,
                WorkOrderId = c.WorkOrderId,
                CreatedAt = c.CreatedAt
            })
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<CostCreateResult> CreateAsync(CreateCostRequest request)
    {
        var workOrderExists = await _db.WorkOrders.AnyAsync(w => w.Id == request.WorkOrderId);

        if (!workOrderExists)
        {
            return new CostCreateResult
            {
                InvalidWorkOrderId = true
            };
        }

        var cost = new Cost
        {
            Name = request.Name,
            Amount = request.Amount,
            WorkOrderId = request.WorkOrderId,
            CreatedAt = DateTime.UtcNow
        };

        _db.Costs.Add(cost);
        await _db.SaveChangesAsync();

        return new CostCreateResult
        {
            Cost = new CostResponse
            {
                Id = cost.Id,
                Name = cost.Name,
                Amount = cost.Amount,
                WorkOrderId = cost.WorkOrderId,
                CreatedAt = cost.CreatedAt
            }
        };
    }

    public async Task<CostUpdateResult> UpdateAsync(int id, UpdateCostRequest request)
    {
        var cost = await _db.Costs.FindAsync(id);

        if (cost == null)
        {
            return new CostUpdateResult
            {
                NotFound = true
            };
        }

        var workOrderExists = await _db.WorkOrders.AnyAsync(w => w.Id == request.WorkOrderId);

        if (!workOrderExists)
        {
            return new CostUpdateResult
            {
                InvalidWorkOrderId = true
            };
        }

        cost.Name = request.Name;
        cost.Amount = request.Amount;
        cost.WorkOrderId = request.WorkOrderId;

        await _db.SaveChangesAsync();

        return new CostUpdateResult
        {
            Cost = new CostResponse
            {
                Id = cost.Id,
                Name = cost.Name,
                Amount = cost.Amount,
                WorkOrderId = cost.WorkOrderId,
                CreatedAt = cost.CreatedAt
            }
        };
    }

    public async Task<CostUpdateResult> PatchAsync(int id, PatchCostRequest request)
    {
        var cost = await _db.Costs.FindAsync(id);

        if (cost == null)
        {
            return new CostUpdateResult
            {
                NotFound = true
            };
        }

        if (request.WorkOrderId.HasValue)
        {
            var workOrderExists = await _db.WorkOrders.AnyAsync(w => w.Id == request.WorkOrderId.Value);

            if (!workOrderExists)
            {
                return new CostUpdateResult
                {
                    InvalidWorkOrderId = true
                };
            }

            cost.WorkOrderId = request.WorkOrderId.Value;
        }

        if (request.Name != null)
        {
            cost.Name = request.Name;
        }

        if (request.Amount.HasValue)
        {
            cost.Amount = request.Amount.Value;
        }

        await _db.SaveChangesAsync();

        return new CostUpdateResult
        {
            Cost = new CostResponse
            {
                Id = cost.Id,
                Name = cost.Name,
                Amount = cost.Amount,
                WorkOrderId = cost.WorkOrderId,
                CreatedAt = cost.CreatedAt
            }
        };
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var cost = await _db.Costs.FindAsync(id);

        if (cost == null)
        {
            return false;
        }

        _db.Costs.Remove(cost);
        await _db.SaveChangesAsync();

        return true;
    }
}