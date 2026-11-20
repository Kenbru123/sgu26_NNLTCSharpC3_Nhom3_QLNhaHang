using Restaurant.Application.DTOs;

namespace Restaurant.Application.Interfaces;

public interface IFoodService
{
    Task<IReadOnlyList<FoodResponse>> GetMenuAsync(
        string? search,
        int? categoryId,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<FoodResponse>> GetAllForManagementAsync(
        CancellationToken cancellationToken);

    Task<IReadOnlyList<FoodCategoryResponse>> GetCategoriesAsync(
        bool includeInactive,
        CancellationToken cancellationToken);

    Task<FoodCategoryResponse> CreateCategoryAsync(
        CreateFoodCategoryRequest request,
        CancellationToken cancellationToken);

    Task<FoodResponse?> GetByIdAsync(
        int id,
        bool includeInactive,
        CancellationToken cancellationToken);

    Task<FoodResponse> CreateAsync(
        CreateFoodRequest request,
        CancellationToken cancellationToken);

    Task<FoodResponse?> UpdateAsync(
        int id,
        UpdateFoodRequest request,
        CancellationToken cancellationToken);

    Task<bool> DeactivateAsync(int id, CancellationToken cancellationToken);
}
