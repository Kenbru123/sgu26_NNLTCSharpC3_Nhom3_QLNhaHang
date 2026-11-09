namespace Restaurant.Application.Exceptions;

public sealed class BanAnInUseException : Exception
{
    public BanAnInUseException()
        : base("Không thể đổi khu vực hoặc ngừng hoạt động khi bàn có đơn hàng hoặc lượt đặt còn hiệu lực.")
    {
    }
}
