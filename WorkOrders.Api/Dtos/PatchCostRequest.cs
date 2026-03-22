namespace WorkOrders.Api.Dtos;

public class PatchCostRequest
{
    public string? Name { get; set; }

    public decimal? Amount { get; set; }

    public int? WorkOrderId { get; set; }
}