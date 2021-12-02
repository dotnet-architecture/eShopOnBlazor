namespace eShopOnBlazor;
public class WebRequestInfo
{
    private readonly string _message;

    public WebRequestInfo(HttpContext context)
    {
        var userAgent = context.Request.Headers["User-Agent"];
        _message = $"{context.Request.Path}, {userAgent}";
    }

    public override string ToString() => _message;
}
