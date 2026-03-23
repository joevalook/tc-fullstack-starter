using Microsoft.EntityFrameworkCore;
using WorkOrders.Api.Data;
using WorkOrders.Api.Dtos;
using WorkOrders.Api.Models;
using WorkOrders.Api.Services;

namespace WorkOrders.Api.Tests.Services;

public class WorkOrderServiceTests
{
    private AppDbContext BuildDbContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task ListAsync_ReturnsItemsOrderedByUpdatedAtDescending()
    {
        // Arrange
        await using var db = BuildDbContext(nameof(ListAsync_ReturnsItemsOrderedByUpdatedAtDescending));

        db.WorkOrders.AddRange(
            new WorkOrder
            {
                Title = "Older work order",
                Description = "First",
                Status = "Open",
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2026, 1, 2, 0, 0, 0, DateTimeKind.Utc)
            },
            new WorkOrder
            {
                Title = "Newest work order",
                Description = "Second",
                Status = "Closed",
                CreatedAt = new DateTime(2026, 1, 3, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2026, 1, 4, 0, 0, 0, DateTimeKind.Utc)
            }
        );

        await db.SaveChangesAsync();

        var service = new WorkOrderService(db);

        // Act
        var result = (await service.ListAsync()).ToList();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal("Newest work order", result[0].Title);
        Assert.Equal("Older work order", result[1].Title);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsWorkOrderWithCosts_WhenFound()
    {
        // Arrange
        await using var db = BuildDbContext(nameof(GetByIdAsync_ReturnsWorkOrderWithCosts_WhenFound));

        var workOrder = new WorkOrder
        {
            Title = "Broken monitor",
            Description = "Screen flickers",
            Status = "Open"
        };

        db.WorkOrders.Add(workOrder);
        await db.SaveChangesAsync();

        db.Costs.AddRange(
            new Cost
            {
                WorkOrderId = workOrder.Id,
                Name = "Replacement cable",
                Amount = 25.50m
            },
            new Cost
            {
                WorkOrderId = workOrder.Id,
                Name = "Labor",
                Amount = 75.00m
            }
        );

        await db.SaveChangesAsync();

        var service = new WorkOrderService(db);

        // Act
        var result = await service.GetByIdAsync(workOrder.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(workOrder.Id, result.Id);
        Assert.Equal("Broken monitor", result.Title);
        Assert.Equal("Screen flickers", result.Description);
        Assert.Equal("Open", result.Status);
        Assert.Equal(2, result.Costs.Count);

        Assert.Contains(result.Costs, c => c.Name == "Replacement cable" && c.Amount == 25.50m);
        Assert.Contains(result.Costs, c => c.Name == "Labor" && c.Amount == 75.00m);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
    {
        // Arrange
        await using var db = BuildDbContext(nameof(GetByIdAsync_ReturnsNull_WhenNotFound));
        var service = new WorkOrderService(db);

        // Act
        var result = await service.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_CreatesWorkOrderAndReturnsResponse()
    {
        // Arrange
        await using var db = BuildDbContext(nameof(CreateAsync_CreatesWorkOrderAndReturnsResponse));
        var service = new WorkOrderService(db);

        var request = new CreateWorkOrderRequest
        {
            Title = "Keyboard issue",
            Description = "Keys sticking",
            Status = "Open"
        };

        // Act
        var result = await service.CreateAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal("Keyboard issue", result.Title);
        Assert.Equal("Keys sticking", result.Description);
        Assert.Equal("Open", result.Status);
        Assert.Empty(result.Costs);

        var itemInDb = await db.WorkOrders.FirstOrDefaultAsync(w => w.Id == result.Id);
        Assert.NotNull(itemInDb);
        Assert.Equal("Keyboard issue", itemInDb.Title);
        Assert.Equal("Keys sticking", itemInDb.Description);
        Assert.Equal("Open", itemInDb.Status);
    }

    [Fact]
    public async Task CreateAsync_UsesEmptyString_WhenDescriptionIsNull()
    {
        // Arrange
        await using var db = BuildDbContext(nameof(CreateAsync_UsesEmptyString_WhenDescriptionIsNull));
        var service = new WorkOrderService(db);

        var request = new CreateWorkOrderRequest
        {
            Title = "Mouse issue",
            Description = null,
            Status = "Open"
        };

        // Act
        var result = await service.CreateAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(string.Empty, result.Description);

        var itemInDb = await db.WorkOrders.FirstOrDefaultAsync(w => w.Id == result.Id);
        Assert.NotNull(itemInDb);
        Assert.Equal(string.Empty, itemInDb.Description);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesAllFields_WhenFound()
    {
        // Arrange
        await using var db = BuildDbContext(nameof(UpdateAsync_UpdatesAllFields_WhenFound));

        var workOrder = new WorkOrder
        {
            Title = "Old title",
            Description = "Old description",
            Status = "Open",
            UpdatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        };

        db.WorkOrders.Add(workOrder);
        await db.SaveChangesAsync();

        var service = new WorkOrderService(db);

        var request = new UpdateWorkOrderRequest
        {
            Title = "New title",
            Description = "New description",
            Status = "Closed"
        };

        // Act
        var result = await service.UpdateAsync(workOrder.Id, request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("New title", result.Title);
        Assert.Equal("New description", result.Description);
        Assert.Equal("Closed", result.Status);
        Assert.True(result.UpdatedAt > new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc));

        var itemInDb = await db.WorkOrders.FirstOrDefaultAsync(w => w.Id == workOrder.Id);
        Assert.NotNull(itemInDb);
        Assert.Equal("New title", itemInDb.Title);
        Assert.Equal("New description", itemInDb.Description);
        Assert.Equal("Closed", itemInDb.Status);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsNull_WhenNotFound()
    {
        // Arrange
        await using var db = BuildDbContext(nameof(UpdateAsync_ReturnsNull_WhenNotFound));
        var service = new WorkOrderService(db);

        var request = new UpdateWorkOrderRequest
        {
            Title = "Anything",
            Description = "Anything",
            Status = "Closed"
        };

        // Act
        var result = await service.UpdateAsync(999, request);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_UsesEmptyString_WhenDescriptionIsNull()
    {
        // Arrange
        await using var db = BuildDbContext(nameof(UpdateAsync_UsesEmptyString_WhenDescriptionIsNull));

        var workOrder = new WorkOrder
        {
            Title = "Initial title",
            Description = "Initial description",
            Status = "Open"
        };

        db.WorkOrders.Add(workOrder);
        await db.SaveChangesAsync();

        var service = new WorkOrderService(db);

        var request = new UpdateWorkOrderRequest
        {
            Title = "Updated title",
            Description = null,
            Status = "In Progress"
        };

        // Act
        var result = await service.UpdateAsync(workOrder.Id, request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(string.Empty, result.Description);

        var itemInDb = await db.WorkOrders.FirstOrDefaultAsync(w => w.Id == workOrder.Id);
        Assert.NotNull(itemInDb);
        Assert.Equal(string.Empty, itemInDb.Description);
    }

    [Fact]
    public async Task PatchAsync_UpdatesOnlyProvidedFields()
    {
        // Arrange
        await using var db = BuildDbContext(nameof(PatchAsync_UpdatesOnlyProvidedFields));

        var workOrder = new WorkOrder
        {
            Title = "Original title",
            Description = "Original description",
            Status = "Open"
        };

        db.WorkOrders.Add(workOrder);
        await db.SaveChangesAsync();

        var originalUpdatedAt = workOrder.UpdatedAt;

        var service = new WorkOrderService(db);

        var request = new PatchWorkOrderRequest
        {
            Title = "Patched title",
            Description = null,
            Status = null
        };

        // Act
        var result = await service.PatchAsync(workOrder.Id, request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Patched title", result.Title);
        Assert.Equal("Original description", result.Description);
        Assert.Equal("Open", result.Status);
        Assert.True(result.UpdatedAt >= originalUpdatedAt);

        var itemInDb = await db.WorkOrders.FirstOrDefaultAsync(w => w.Id == workOrder.Id);
        Assert.NotNull(itemInDb);
        Assert.Equal("Patched title", itemInDb.Title);
        Assert.Equal("Original description", itemInDb.Description);
        Assert.Equal("Open", itemInDb.Status);
    }

    [Fact]
    public async Task PatchAsync_UpdatesDescription_WhenProvided()
    {
        // Arrange
        await using var db = BuildDbContext(nameof(PatchAsync_UpdatesDescription_WhenProvided));

        var workOrder = new WorkOrder
        {
            Title = "Original title",
            Description = "Original description",
            Status = "Open"
        };

        db.WorkOrders.Add(workOrder);
        await db.SaveChangesAsync();

        var service = new WorkOrderService(db);

        var request = new PatchWorkOrderRequest
        {
            Description = "Updated only description"
        };

        // Act
        var result = await service.PatchAsync(workOrder.Id, request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Original title", result.Title);
        Assert.Equal("Updated only description", result.Description);
        Assert.Equal("Open", result.Status);
    }

    [Fact]
    public async Task PatchAsync_ReturnsNull_WhenNotFound()
    {
        // Arrange
        await using var db = BuildDbContext(nameof(PatchAsync_ReturnsNull_WhenNotFound));
        var service = new WorkOrderService(db);

        var request = new PatchWorkOrderRequest
        {
            Title = "Anything"
        };

        // Act
        var result = await service.PatchAsync(999, request);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteAsync_RemovesWorkOrder_WhenFound()
    {
        // Arrange
        await using var db = BuildDbContext(nameof(DeleteAsync_RemovesWorkOrder_WhenFound));

        var workOrder = new WorkOrder
        {
            Title = "Delete me",
            Description = "To be removed",
            Status = "Open"
        };

        db.WorkOrders.Add(workOrder);
        await db.SaveChangesAsync();

        var service = new WorkOrderService(db);

        // Act
        var result = await service.DeleteAsync(workOrder.Id);

        // Assert
        Assert.True(result);

        var itemInDb = await db.WorkOrders.FirstOrDefaultAsync(w => w.Id == workOrder.Id);
        Assert.Null(itemInDb);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsFalse_WhenNotFound()
    {
        // Arrange
        await using var db = BuildDbContext(nameof(DeleteAsync_ReturnsFalse_WhenNotFound));
        var service = new WorkOrderService(db);

        // Act
        var result = await service.DeleteAsync(999);

        // Assert
        Assert.False(result);
    }
}