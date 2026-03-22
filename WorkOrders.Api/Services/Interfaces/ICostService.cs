using WorkOrders.Api.Dtos;

namespace WorkOrders.Api.Services.Interfaces;

public interface ICostService
{
    Task<IEnumerable<CostResponse>> ListAsync();
    Task<CostResponse?> GetByIdAsync(int id);
    Task<CostCreateResult> CreateAsync(CreateCostRequest request);
    Task<CostUpdateResult> UpdateAsync(int id, UpdateCostRequest request);
    Task<CostUpdateResult> PatchAsync(int id, PatchCostRequest request);
    Task<bool> DeleteAsync(int id);
}

public class CostCreateResult
{
    public bool InvalidWorkOrderId { get; set; }
    public CostResponse? Cost { get; set; }
}

public class CostUpdateResult
{
    public bool NotFound { get; set; }
    public bool InvalidWorkOrderId { get; set; }
    public CostResponse? Cost { get; set; }
}