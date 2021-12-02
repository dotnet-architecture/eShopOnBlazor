namespace eShopOnBlazor;
public class ActivityIdHelper
{
    private readonly string _activityId;

    public ActivityIdHelper(HttpContext ctx)
    {
        _activityId = ctx.TraceIdentifier;
    }

    public override string ToString() => _activityId;
}
