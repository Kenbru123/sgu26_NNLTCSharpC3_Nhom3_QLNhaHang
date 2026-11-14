using Restaurant.Application.DTOs;

namespace Restaurant.Web.Models;

public sealed class MenuViewModel
{
    public required IReadOnlyList<MonAnDto> Foods { get; init; }
    public required IReadOnlyList<FoodCategoryResponse> Categories { get; init; }
    public string? Search { get; init; }
    public int? CategoryId { get; init; }
}
