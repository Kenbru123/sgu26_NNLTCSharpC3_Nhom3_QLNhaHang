using Restaurant.Application.DTOs;

namespace Restaurant.Web.Models;

public sealed record CartLineViewModel(
    int FoodId,
    int Quantity,
    MonAnDto? Food)
{
    public decimal LineTotal => Food is null ? 0m : Food.DonGia * Quantity;
}

public sealed class CartViewModel
{
    public required IReadOnlyList<CartLineViewModel> Items { get; init; }
    public decimal Total => Items.Sum(item => item.LineTotal);
}
