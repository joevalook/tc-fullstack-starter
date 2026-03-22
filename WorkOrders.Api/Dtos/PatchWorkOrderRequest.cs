using System.ComponentModel.DataAnnotations;

namespace WorkOrders.Api.Dtos;

public class PatchWorkOrderRequest
{
    [MaxLength(500)]
    public string? Description { get; set; }

    public string? Title { get; set; }

    public string? Status { get; set; }
}