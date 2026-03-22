namespace WorkOrders.Api.Models;

public class Cost
{
    public int Id { get; set; }

    public int WorkOrderId { get; set; }

    public string Name { get; set; } = "";

    public decimal Amount { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public WorkOrder? WorkOrder { get; set; }
}