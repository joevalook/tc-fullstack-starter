using System.ComponentModel.DataAnnotations;

namespace WorkOrders.Api.Dtos;

public class UpdateCostRequest
{
    [Required]
    public string Name {get; set;} = "";

    [Range(0, double.MaxValue)]
    public decimal Amount {get; set;}

    [Required]
    public int WorkOrderId {get; set;}
}