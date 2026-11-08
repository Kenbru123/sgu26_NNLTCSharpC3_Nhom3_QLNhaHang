namespace Restaurant.Application.Exceptions;

public sealed class DuplicateKhuVucNameException : Exception
{
    public DuplicateKhuVucNameException(string name, Exception? innerException = null)
        : base($"Khu vực '{name}' đã tồn tại.", innerException)
    {
    }
}
