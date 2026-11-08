namespace Restaurant.Application.Exceptions;

public sealed class InvalidBanAreaException : Exception
{
    public InvalidBanAreaException(int areaId, Exception? innerException = null)
        : base($"Khu vực với mã {areaId} không tồn tại.", innerException)
    {
    }
}
