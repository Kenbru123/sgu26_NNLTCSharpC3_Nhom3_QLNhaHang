namespace Restaurant.Application.Exceptions;

public sealed class GoiMonNotFoundException : Exception
{
    public GoiMonNotFoundException(string message) : base(message)
    {
    }
}

public sealed class GoiMonConflictException : Exception
{
    public GoiMonConflictException(string message) : base(message)
    {
    }
}
