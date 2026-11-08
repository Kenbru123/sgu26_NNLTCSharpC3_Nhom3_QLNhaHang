namespace Restaurant.Application.Exceptions;

public sealed class DuplicateBanNameException : Exception
{
    public DuplicateBanNameException(string name, Exception? innerException = null)
        : base($"Tên bàn '{name}' đã tồn tại trong khu vực đã chọn.", innerException)
    {
    }
}
