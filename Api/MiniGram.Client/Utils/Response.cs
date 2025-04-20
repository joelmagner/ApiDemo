namespace MiniGram.Client.Utils;

public abstract class Response<T> : Response
{
    protected Response() { }
    protected Response(string error, int httpStatus = 400) : base(error, httpStatus) { }
    protected Response(T item, int httpStatus = 200)
    {
        Item = item;
        HttpStatus = httpStatus;
    }


    public T Item { get; init; } = default!;
}

public abstract class Response
{
    protected Response() { }
    protected Response(string error = "", int httpStatus = 200)
    {
        Error = error;
        HttpStatus = httpStatus;
    }

    public virtual int HttpStatus { get; init; }
    public string Error { get; init; } = string.Empty;
    public bool IsSuccess => string.IsNullOrWhiteSpace(Error);
}