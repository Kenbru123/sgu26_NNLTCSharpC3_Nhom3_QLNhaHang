namespace Restaurant.Web.Services;

public sealed record CartItem(int MaMon, int SoLuong);

public interface ICartService
{
    IReadOnlyList<CartItem> GetItems();
    void Add(int foodId, int quantity);
    void SetQuantity(int foodId, int quantity);
    void Remove(int foodId);
    void Clear();
}
