namespace WorkOrders.Api.Dtos;

public class CostResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public decimal Amount { get; set; }
    public int WorkOrderId { get; set; }
    public DateTime CreatedAt { get; set; }
}