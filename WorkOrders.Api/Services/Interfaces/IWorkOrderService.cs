using WorkOrders.Api.Dtos;

namespace WorkOrders.Api.Services.Interfaces;

public interface IWorkOrderService
{
    Task<IEnumerable<WorkOrderListItemResponse>> ListAsync();
    Task<WorkOrderResponse?> GetByIdAsync(int id);
    Task<WorkOrderResponse> CreateAsync(CreateWorkOrderRequest request);
    Task<WorkOrderResponse?> UpdateAsync(int id, UpdateWorkOrderRequest request);
    Task<WorkOrderResponse?> PatchAsync(int id, PatchWorkOrderRequest request);
    Task<bool> DeleteAsync(int id);
}