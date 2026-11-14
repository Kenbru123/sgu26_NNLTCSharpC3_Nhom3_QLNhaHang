using System.Text.Json;

namespace Restaurant.Web.Services;

public sealed class CartService : ICartService
{
    private const string SessionKey = "restaurant-cart";
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CartService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public IReadOnlyList<CartItem> GetItems() =>
        ReadCart()
            .OrderBy(item => item.Key)
            .Select(item => new CartItem(item.Key, item.Value))
            .ToList();

    public void Add(int foodId, int quantity)
    {
        if (foodId <= 0 || quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity));
        }

        var cart = ReadCart();
        cart.TryGetValue(foodId, out var currentQuantity);
        cart[foodId] = Math.Min(currentQuantity + quantity, 99);
        WriteCart(cart);
    }

    public void SetQuantity(int foodId, int quantity)
    {
        if (foodId <= 0 || quantity < 1 || quantity > 99)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity));
        }

        var cart = ReadCart();
        if (!cart.ContainsKey(foodId))
        {
            return;
        }

        cart[foodId] = quantity;
        WriteCart(cart);
    }

    public void Remove(int foodId)
    {
        var cart = ReadCart();
        cart.Remove(foodId);
        WriteCart(cart);
    }

    public void Clear() => Session.Remove(SessionKey);

    private Dictionary<int, int> ReadCart()
    {
        var json = Session.GetString(SessionKey);
        return string.IsNullOrWhiteSpace(json)
            ? new Dictionary<int, int>()
            : JsonSerializer.Deserialize<Dictionary<int, int>>(json)
                ?? new Dictionary<int, int>();
    }

    private void WriteCart(Dictionary<int, int> cart) =>
        Session.SetString(SessionKey, JsonSerializer.Serialize(cart));

    private ISession Session =>
        _httpContextAccessor.HttpContext?.Session
        ?? throw new InvalidOperationException(
            "Không thể truy cập phiên giỏ hàng ngoài HTTP request.");
}
