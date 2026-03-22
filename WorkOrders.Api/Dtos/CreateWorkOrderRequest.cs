using System.ComponentModel.DataAnnotations;

namespace WorkOrders.Api.Dtos;

public class CreateWorkOrderRequest
{
    [Required]
    public string Title {get; set;} = "";

    [MaxLength(500)]
    public string? Description {get; set;}

    [Required]
    public string Status { get; set;} = "";
}